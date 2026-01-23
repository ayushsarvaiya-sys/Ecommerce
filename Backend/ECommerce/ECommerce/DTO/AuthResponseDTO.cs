using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class AuthResponseDTO
    {
        public int Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }
    }
}
