using ECommerce.DTO;
using ECommerce.Models;

namespace ECommerce.Interfaces
{
    public interface IAuthRepository
    {
        public Task<UserModel> AddUser(UserModel user);

        public Task<UserModel?> GetUserByEmail(string email);
    }
}
