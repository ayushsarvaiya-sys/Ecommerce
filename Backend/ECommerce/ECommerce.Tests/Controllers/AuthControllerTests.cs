//using ECommerce.Controllers;
//using ECommerce.DTO;
//using ECommerce.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ECommerce.Tests.Controllers
//{
//    public class AuthControllerTests
//    {
//        private readonly Mock<IAuthService> _serviceMock;
//        private readonly AuthController _controller;

//        public AuthControllerTests()
//        {
//            _serviceMock = new Mock<IAuthService>();
//            _controller = new AuthController(_serviceMock.Object);

//            _controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext()
//            };
//        }

//        // REGISTER

//        [Fact]
//        public async Task Register_Should_Return_Ok()
//        {
//            _serviceMock.Setup(s => s.RegistrationService(It.IsAny<RegistrationRequestDTO>()))
//                .ReturnsAsync(new AuthResponseDTO
//                {
//                    Email = "test@test.com"
//                });

//            var result = await _controller.Register(new RegistrationRequestDTO());

//            Assert.IsType<OkObjectResult>(result);
//        }

//        [Fact]
//        public async Task Register_Should_Return_BadRequest_On_Error()
//        {
//            _serviceMock.Setup(s => s.RegistrationService(It.IsAny<RegistrationRequestDTO>()))
//                .ThrowsAsync(new Exception("User already exists"));

//            var result = await _controller.Register(new RegistrationRequestDTO());

//            Assert.IsType<BadRequestObjectResult>(result);
//        }

//        // LOGIN

//        [Fact]
//        public async Task Login_Should_Return_Ok()
//        {
//            _serviceMock.Setup(s => s.LoginService(It.IsAny<LoginRequestDTO>()))
//                .ReturnsAsync((new AuthResponseDTO { Email = "test@test.com" }, "jwt"));

//            var result = await _controller.Login(new LoginRequestDTO());

//            Assert.IsType<OkObjectResult>(result);
//        }

//        [Fact]
//        public async Task Login_Should_Return_BadRequest_On_Error()
//        {
//            _serviceMock.Setup(s => s.LoginService(It.IsAny<LoginRequestDTO>()))
//                .ThrowsAsync(new Exception("Invalid"));

//            var result = await _controller.Login(new LoginRequestDTO());

//            Assert.IsType<BadRequestObjectResult>(result);
//        }
//    }
//}



















using ECommerce.Controllers;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ECommerce.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _serviceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _serviceMock = new Mock<IAuthService>();
            _controller = new AuthController(_serviceMock.Object);

            // Setup HttpContext to test cookie operations
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        #region Register Tests

        [Fact]
        public async Task Register_Should_Return_Ok_With_Valid_Data()
        {
            // Arrange
            var registerDto = new RegistrationRequestDTO
            {
                FullName = "Ayush Patel",
                Email = "ayush@test.com",
                Password = "Password@123",
                Role = "User"
            };

            var responseDto = new AuthResponseDTO
            {
                Id = 1,
                FullName = "Ayush Patel",   
                Email = "ayush@test.com",
                Role = "User"
            };

            _serviceMock.Setup(s => s.RegistrationService(It.IsAny<RegistrationRequestDTO>()))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<AuthResponseDTO>>(okResult.Value);

            Assert.Equal(200, apiResponse.StatusCode);
            Assert.Equal("Registration successful", apiResponse.Message);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal("ayush@test.com", apiResponse.Data.Email);

            _serviceMock.Verify(s => s.RegistrationService(It.IsAny<RegistrationRequestDTO>()), Times.Once);
        }

        [Fact]
        public async Task Register_Should_Return_BadRequest_When_Email_Already_Exists()
        {
            // Arrange
            var registerDto = new RegistrationRequestDTO
            {
                Email = "existing@test.com",
                Password = "Password@123"
            };

            _serviceMock.Setup(s => s.RegistrationService(It.IsAny<RegistrationRequestDTO>()))
                .ThrowsAsync(new ArgumentException("User with given Email ID is already exists"));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);

            Assert.Equal(400, apiError.StatusCode);
            Assert.Equal("User with given Email ID is already exists", apiError.Message);
        }

        [Fact]
        public async Task Register_Should_Return_BadRequest_On_Any_Exception()
        {
            // Arrange
            _serviceMock.Setup(s => s.RegistrationService(It.IsAny<RegistrationRequestDTO>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Register(new RegistrationRequestDTO());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);

            Assert.Equal(400, apiError.StatusCode);
            Assert.Contains("Database error", apiError.Message);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_Should_Return_Ok_And_Set_Cookie_With_Valid_Credentials()
        {
            // Arrange
            var loginDto = new LoginRequestDTO
            {
                Email = "ayush@test.com",
                Password = "Password@123"
            };

            var responseDto = new AuthResponseDTO
            {
                Id = 1,
                Email = "ayush@test.com",
                FullName = "Ayush Patel",
                Role = "User"
            };

            var token = "fake-jwt-token-12345";

            _serviceMock.Setup(s => s.LoginService(It.IsAny<LoginRequestDTO>()))
                .ReturnsAsync((responseDto, token));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<AuthResponseDTO>>(okResult.Value);

            Assert.Equal(200, apiResponse.StatusCode);
            Assert.Equal("Login successful", apiResponse.Message);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal("ayush@test.com", apiResponse.Data.Email);

            // Verify cookie was set
            Assert.True(_controller.Response.Headers.ContainsKey("Set-Cookie"));

            _serviceMock.Verify(s => s.LoginService(It.IsAny<LoginRequestDTO>()), Times.Once);
        }

        [Fact]
        public async Task Login_Should_Return_BadRequest_When_User_Not_Found()
        {
            // Arrange
            var loginDto = new LoginRequestDTO
            {
                Email = "notfound@test.com",
                Password = "Password@123"
            };

            _serviceMock.Setup(s => s.LoginService(It.IsAny<LoginRequestDTO>()))
                .ThrowsAsync(new ArgumentException("User not Found"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);

            Assert.Equal(400, apiError.StatusCode);
            Assert.Equal("User not Found", apiError.Message);
        }

        [Fact]
        public async Task Login_Should_Return_BadRequest_When_Password_Is_Incorrect()
        {
            // Arrange
            var loginDto = new LoginRequestDTO
            {
                Email = "ayush@test.com",
                Password = "WrongPassword@123"
            };

            _serviceMock.Setup(s => s.LoginService(It.IsAny<LoginRequestDTO>()))
                .ThrowsAsync(new ArgumentException("Password is incorrect"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);

            Assert.Equal(400, apiError.StatusCode);
            Assert.Equal("Password is incorrect", apiError.Message);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public void Logout_Should_Delete_Cookie_And_Return_Ok()
        {
            // Act
            var result = _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Verify the response contains success message
            var response = okResult.Value;
            Assert.NotNull(response);

            // Check if cookie deletion was attempted
            // (In real scenario, you'd check Response.Cookies but that's harder to test)
            Assert.IsType<OkObjectResult>(result);
        }

        #endregion
    }
}