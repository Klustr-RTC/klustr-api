using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Klustr_api.Data;
using Klustr_api.Dtos.User;
using Klustr_api.Interfaces;
using Klustr_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Klustr_api.Repository
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationDBContext _context;
        private readonly ITokenService _tokenService;

        public UserRepository(ApplicationDBContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string?> Register(UserRegistrationDto userRegistrationDto)
        {
            var user = new User
            {
                Id = new Guid(),
                Username = userRegistrationDto.Username,
                Email = userRegistrationDto.Email
            };

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password);

            user.PasswordHash = hashedPassword;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return _tokenService.CreateToken(user.Email, user.Username);
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}