using ECommerce.Database;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Models;
using ECommerce.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EcommerceDbContext _context;
        public AuthRepository(EcommerceDbContext context) 
        {
            _context = context;
        }

        public async Task<UserModel> AddUser(UserModel user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<UserModel?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
