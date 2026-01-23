using AutoMapper;
using ECommerce.DTO;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce.Tests.Helpers
{
    public class AutoMapperTestProfile : Profile
    {
        public AutoMapperTestProfile()
        {
            CreateMap<UserModel, AuthResponseDTO>();
            CreateMap<RegistrationRequestDTO, UserModel>();
        }
    }
}
