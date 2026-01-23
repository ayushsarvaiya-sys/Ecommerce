using AutoMapper;
using ECommerce.DTO;
using ECommerce.Interfaces;
using ECommerce.Models;
using ECommerce.Utils;

namespace ECommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IAuthRepository authRepository, IMapper mapper, IJwtTokenGenerator jwtTokenGenerator) 
        {
            _authRepository = authRepository;
            _mapper = mapper;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<(AuthResponseDTO, string)> LoginService(LoginRequestDTO user)
        {
            // Find user by email
            var findUser = await _authRepository.GetUserByEmail(user.Email);  

            if (findUser == null)
            {
                throw new ArgumentException("User not Found");
            }

            // Verify Password
            bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(user.Password, findUser.Password);

            if (!isPasswordValid)
            {
                throw new ArgumentException("Password is incorrect");
            }

            var response = _mapper.Map<AuthResponseDTO>(findUser);

            string token = _jwtTokenGenerator.GenerateToken(findUser);

            return (response, token);
        }

        public async Task<AuthResponseDTO> RegistrationService(RegistrationRequestDTO user)
        {
            var findUser = await _authRepository.GetUserByEmail(user.Email);

            if (findUser != null)
            {
                throw new ArgumentException("User with given Email ID is already exists");
            }

            var newUSer = _mapper.Map<UserModel>(user);

            // Hash the Password
            newUSer.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newUSer.Password, 13);

            UserModel res = await _authRepository.AddUser(newUSer);

            return _mapper.Map<AuthResponseDTO>(res);
        }
    }
}
