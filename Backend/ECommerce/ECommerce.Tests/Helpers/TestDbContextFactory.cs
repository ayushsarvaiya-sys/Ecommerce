using ECommerce.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Tests.Helpers
{
    internal class TestDbContextFactory
    {
        public static EcommerceDbContext Create()
        {
            var options = new DbContextOptionsBuilder<EcommerceDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new EcommerceDbContext(options);
        }
    }
}
