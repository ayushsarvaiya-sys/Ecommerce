using ECommerce.Models;
using ECommerce.Repositories;
using ECommerce.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Tests.Repositories
{
    public class AuthRepositoryTests
    {
        [Fact]
        public async Task AddUser_Should_Save_User()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var repo = new AuthRepository(context);

            var user = new UserModel
            {
                FullName = "Ayush",
                Email = "test@test.com",
                Password = "hashed",
                Role = "User"
            };

            // Act
            var result = await repo.AddUser(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task GetUserByEmail_Should_Return_User()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            context.Users.Add(new UserModel
            {
                FullName = "Ayush",
                Email = "test@test.com",
                Password = "hashed",
                Role = "User"
            });
            await context.SaveChangesAsync();

            var repo = new AuthRepository(context);

            // Act
            var user = await repo.GetUserByEmail("test@test.com");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("test@test.com", user.Email);
        }
    }
}
