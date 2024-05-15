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

        public async Task<User?> FindByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<User?> FindByUsername(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        public async Task<string?> GoogleAuth(GoogleAuthDto googleAuthDto)
        {
            var user = await FindByEmail(googleAuthDto.Email);

            if (user == null)
            {
                var user1 = await FindByUsername(googleAuthDto.Username);
                if (user1 != null)
                {
                    return null;
                }
                user = new User
                {
                    Id = new Guid(),
                    Email = googleAuthDto.Email,
                    Username = googleAuthDto.Username,
                    GoogleId = googleAuthDto.GoogleId,
                    GoogleAccessToken = googleAuthDto.GoogleAccessToken,
                    GoogleRefreshToken = googleAuthDto.GoogleRefreshToken
                };

                await _context.Users.AddAsync(user);
            }
            else
            {
                user.GoogleId = googleAuthDto.GoogleId;
                user.GoogleAccessToken = googleAuthDto.GoogleAccessToken;
                user.GoogleRefreshToken = googleAuthDto.GoogleRefreshToken;
            }
            await _context.SaveChangesAsync();
            return _tokenService.CreateToken(user.Id, user.Email, user.Username);
        }

        public async Task<string?> Login(UserLoginDto userLoginDto)
        {
            var user = await FindByEmail(userLoginDto.Email);

            if (user == null)
            {
                return null;
            }

            if (user.PasswordHash == null)
            {
                return null;
            }
            var isPassCorrect = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash);

            if (!isPassCorrect)
            {
                return null;
            }

            return _tokenService.CreateToken(user.Id, user.Email, user.Username);
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

            return _tokenService.CreateToken(user.Id, user.Email, user.Username);
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}