using ECommerce.DTO;

namespace ECommerce.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponseDTO> RegistrationService(RegistrationRequestDTO user);

        public Task<(AuthResponseDTO, string)> LoginService(LoginRequestDTO user);
    }
}
