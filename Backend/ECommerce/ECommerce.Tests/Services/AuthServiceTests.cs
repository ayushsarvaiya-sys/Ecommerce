//using AutoMapper;
//using ECommerce.DTO;
//using ECommerce.Interfaces;
//using ECommerce.Models;
//using ECommerce.Services;
//using ECommerce.Utils;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ECommerce.Tests.Services
//{
//    public class AuthServiceTests
//    {
//        private readonly Mock<IAuthRepository> _repoMock;
//        private readonly Mock<IMapper> _mapperMock;
//        private readonly Mock<IJwtTokenGenerator> _jwtMock;
//        private readonly AuthService _service;

//        public AuthServiceTests()
//        {
//            _repoMock = new Mock<IAuthRepository>();
//            _mapperMock = new Mock<IMapper>();
//            _jwtMock = new Mock<IJwtTokenGenerator>();

//            _service = new AuthService(
//                _repoMock.Object,
//                _mapperMock.Object,
//                _jwtMock.Object
//            );
//        }

//        [Fact]
//        public async Task LoginService_Should_Return_Token_And_User()
//        {
//            var password = "Password@123";
//            var hashed = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

//            var user = new UserModel
//            {
//                Id = 1,
//                Email = "test@test.com",
//                Password = hashed,
//                FullName = "Ayush",
//                Role = "User"
//            };

//            var userResponse = new AuthResponseDTO
//            {
//                Id = 1,
//                Email = "test@test.com",
//                FullName = "Ayush",
//                Role = "User"
//            };

//            _repoMock.Setup(r => r.GetUserByEmail(user.Email))
//                     .ReturnsAsync(user);

//            _mapperMock.Setup(m => m.Map<AuthResponseDTO>(user))
//                       .Returns(userResponse);

//            _jwtMock.Setup(j => j.GenerateToken(user))
//                    .Returns("fake-jwt");

//            var (response, token) = await _service.LoginService(
//                new LoginRequestDTO
//                {
//                    Email = user.Email,
//                    Password = password
//                });

//            Assert.Equal("fake-jwt", token);
//            Assert.Equal(user.Email, response.Email);
//        }
//    }
//}











using AutoMapper;
using BCrypt.Net;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Models;
using ECommerce.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ECommerce.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IJwtTokenGenerator> _jwtMock;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _repoMock = new Mock<IAuthRepository>();
            _mapperMock = new Mock<IMapper>();
            _jwtMock = new Mock<IJwtTokenGenerator>();

            _service = new AuthService(
                _repoMock.Object,
                _mapperMock.Object,
                _jwtMock.Object
            );
        }

        // LOGIN TESTS 

        [Fact]
        public async Task LoginService_Should_Return_Token_And_User()
        {
            // Arrange
            var password = "Password@123";
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

            var user = new UserModel
            {
                Id = 1,
                Email = "test@test.com",
                Password = hashedPassword,
                FullName = "Ayush",
                Role = "User"
            };

            var responseDto = new AuthResponseDTO
            {
                Id = 1,
                Email = "test@test.com",
                FullName = "Ayush",
                Role = "User"
            };

            _repoMock.Setup(r => r.GetUserByEmail(user.Email))
                     .ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<AuthResponseDTO>(user))
                       .Returns(responseDto);

            _jwtMock.Setup(j => j.GenerateToken(user))
                    .Returns("fake-jwt");

            // Act
            var (response, token) = await _service.LoginService(
                new LoginRequestDTO
                {
                    Email = user.Email,
                    Password = password
                });

            // Assert
            Assert.Equal("fake-jwt", token);
            Assert.Equal(user.Email, response.Email);
        }

        [Fact]
        public async Task LoginService_Should_Throw_When_User_Not_Found()
        {
            _repoMock.Setup(r => r.GetUserByEmail(It.IsAny<string>()))
                     .ReturnsAsync((UserModel)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.LoginService(new LoginRequestDTO
                {
                    Email = "notfound@test.com",
                    Password = "Password@123"
                }));
        }

        [Fact]
        public async Task LoginService_Should_Throw_When_Password_Is_Wrong()
        {
            var user = new UserModel
            {
                Email = "test@test.com",
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Correct@123")
            };

            _repoMock.Setup(r => r.GetUserByEmail(user.Email))
                     .ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.LoginService(new LoginRequestDTO
                {
                    Email = user.Email,
                    Password = "Wrong@123"
                }));
        }

        // REGISTRATION TESTS 

        [Fact]
        public async Task RegistrationService_Should_Create_User_When_Email_Not_Exists()
        {
            // Arrange
            var registerDto = new RegistrationRequestDTO
            {
                FullName = "Ayush",
                Email = "test@test.com",
                Password = "Password@123",
                Role = "User"
            };

            var mappedUser = new UserModel
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Password = registerDto.Password,
                Role = registerDto.Role
            };

            var savedUser = new UserModel
            {
                Id = 1,
                FullName = "Ayush",
                Email = "test@test.com",
                Password = "hashed-password",
                Role = "User"
            };

            var responseDto = new AuthResponseDTO
            {
                Id = 1,
                FullName = "Ayush",
                Email = "test@test.com",
                Role = "User"
            };

            _repoMock.Setup(r => r.GetUserByEmail(registerDto.Email))
                     .ReturnsAsync((UserModel)null);

            _mapperMock.Setup(m => m.Map<UserModel>(registerDto))
                       .Returns(mappedUser);

            _repoMock.Setup(r => r.AddUser(It.IsAny<UserModel>()))
                     .ReturnsAsync(savedUser);

            _mapperMock.Setup(m => m.Map<AuthResponseDTO>(savedUser))
                       .Returns(responseDto);

            // Act
            var result = await _service.RegistrationService(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(registerDto.Email, result.Email);

            // Password must be hashed
            Assert.NotEqual(registerDto.Password, mappedUser.Password);
            Assert.True(
                BCrypt.Net.BCrypt.EnhancedVerify(
                    registerDto.Password,
                    mappedUser.Password
                )
            );

            _repoMock.Verify(r => r.AddUser(It.IsAny<UserModel>()), Times.Once);
        }

        [Fact]
        public async Task RegistrationService_Should_Throw_When_Email_Already_Exists()
        {
            var registerDto = new RegistrationRequestDTO
            {
                Email = "test@test.com",
                Password = "Password@123"
            };

            _repoMock.Setup(r => r.GetUserByEmail(registerDto.Email))
                     .ReturnsAsync(new UserModel());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.RegistrationService(registerDto));
        }
    }
}
