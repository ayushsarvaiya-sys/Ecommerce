using AutoMapper;
using ECommerce.DTO;
using ECommerce.Models;

namespace ECommerce.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            // Auth Mappings
            CreateMap<RegistrationRequestDTO, UserModel>();
            CreateMap<UserModel, AuthResponseDTO>();

            // Category Mappings
            CreateMap<CreateCategoryDTO, CategoryModel>();
            CreateMap<CategoryResponseDTO, CategoryModel>();
            CreateMap<CategoryModel, CategoryResponseDTO>();

            // Product Mappings
            CreateMap<CreateProductDTO, ProductModel>();
            CreateMap<ProductDTO, ProductModel>();
            CreateMap<ProductModel, ProductDTO>();
            
            // Product to Response DTO with category mapping
            CreateMap<ProductModel, ProductResponseDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.StockStatus, opt => opt.Ignore()); // StockStatus is calculated in service
        }
    }
}
