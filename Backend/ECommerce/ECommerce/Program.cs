using ECommerce.Config;
using ECommerce.Database;
using ECommerce.Interfaces;
using ECommerce.Mappings;
using ECommerce.Middleware;
using ECommerce.Repositories;
using ECommerce.Services;
using ECommerce.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Threading.RateLimiting;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200", "https://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

// DbContext
builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    // READ JWT FROM COOKIE
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AccessToken"];
            return Task.CompletedTask;
        }
    };
});



// Model Validation => ModelState error handeling
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(new ApiError(
            (int)HttpStatusCode.BadRequest,
            "Validation Failed",
            errors
        ));
    };
});

// Mapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
});

// Response Compression - Brotli (Primary) & Gzip (Fallback)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    
    options.ExcludedMimeTypes = new[]
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "application/octet-stream"
    };
    
    // Expanded MIME types for compression
    var mimeTypes = new[] 
    { 
        "application/json",
        "text/plain",
        "text/xml",
        "application/xml",
        "text/csv",
        "image/svg+xml",
        "text/javascript",
        "application/javascript"
    };
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(mimeTypes);
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    // Valid CompressionLevel values: NoCompression(0), Fastest(1), Optimal(2), SmallestSize(3)
    // Using Optimal for balanced compression speed and ratio
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});




// Configure Cloudinary Settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

// DI registration
// Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Cloudinary
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Category
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Product
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductBulkService, ProductBulkService>();

// Cart
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Rate Limiting - Token Bucket Algorithm (10 requests per minute per IP)
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("api-limiter", context =>
        RateLimitPartition.GetTokenBucketLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 10,
                TokensPerPeriod = 10,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                AutoReplenishment = true
            }));

    options.OnRejected = async (context, _) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue)
            ? ((TimeSpan)retryAfterValue).TotalSeconds
            : 60;

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            statusCode = StatusCodes.Status429TooManyRequests,
            message = "Rate limit exceeded. Too many requests.",
            retryAfter = retryAfter
        });
    };
});

// Build the app
var app = builder.Build();

// Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// IMPORTANT: Response Compression MUST come first
app.UseResponseCompression();

app.UseHttpsRedirection();

// CORS must come before Rate Limiter (so 429 responses include CORS headers)
app.UseCors("CorsPolicy");

// Rate Limiting (must be after CORS to include CORS headers in 429 responses)
app.UseRateLimiter();

// database Seeder
using (var scope = app.Services.CreateScope())  
{
    var db = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
