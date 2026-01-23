using ECommerce.Models;

namespace ECommerce.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(UserModel user);
    }
}
