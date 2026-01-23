using Azure.Core;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Models;
using ECommerce.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) 
        {
            _authService = authService;
        }   

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO user)
        {
            try
            {
                var (responseDTO, token) = await _authService.LoginService(user);

                // Store JWT in HttpOnly cookie - PERSISTENT & CROSS-ORIGIN COMPATIBLE
                Response.Cookies.Append("AccessToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,                              // Must be HTTPS
                    SameSite = SameSiteMode.None,               // For cross-origin
                    Expires = DateTime.UtcNow.AddDays(7),       // Makes cookie persistent
                    Path = "/",
                    Domain = null
                });

                return Ok(new ApiResponse<AuthResponseDTO>(200, responseDTO, "Login successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO user)
        {
            try
            {
                AuthResponseDTO authResponseDTO = await _authService.RegistrationService(user);

                return Ok(new ApiResponse<AuthResponseDTO>(200, authResponseDTO, "Registration successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(400, ex.Message));
            }
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AccessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,                  
                SameSite = SameSiteMode.None,   
                Path = "/",
                Domain = null
            });

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
