using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using TodoApp.Api.Application.DTOs;
using TodoApp.Api.Core.Entities;
using TodoApp.Api.Core.Interfaces;

namespace TodoApp.Api.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new ArgumentException("Passwords do not match.");

            if (!await _userRepository.IsEmailUniqueAsync(dto.Email))
                throw new ArgumentException("Email is already taken.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                CreatedAt = DateTime.Now
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var createdUser = await _userRepository.CreateUserAsync(user);

            var token = _jwtProvider.Generate(createdUser);

            return new AuthResponseDto
            {
                Token = token,
                Name = createdUser.Name,
                Email = createdUser.Email
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var token = _jwtProvider.Generate(user);

            return new AuthResponseDto
            {
                Token = token,
                Name = user.Name,
                Email = user.Email
            };
        }
    }
}
