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

        public async Task<ErrorOrToken> GoogleAuth(GoogleAuthDto googleAuthDto)
        {
            var userInfo = await _tokenService.GetUserInfoFromAccessToken(googleAuthDto.GoogleAccessToken);
            if (userInfo == null)
            {
                return new ErrorOrToken { Error = "Invalid access token" };
            }
            if (userInfo.EmailVerified == false)
            {
                return new ErrorOrToken { Error = "Email not verified" };
            }
            var user = await FindByEmail(userInfo.Email);
            if (user == null)
            {
                var user1 = await FindByUsername(googleAuthDto.Username);
                if (user1 != null)
                {
                    return new ErrorOrToken { Error = "Username already taken" };
                }
                user = new User
                {
                    Id = new Guid(),
                    Username = googleAuthDto.Username,
                    Email = userInfo.Email
                };
                await _context.Users.AddAsync(user);
            }
            await _context.SaveChangesAsync();
            var token = _tokenService.CreateToken(user.Id, user.Email, user.Username);
            return new ErrorOrToken { Token = token };
        }

        public async Task<ErrorOrToken> Login(UserLoginDto userLoginDto)
        {
            var user = await FindByEmail(userLoginDto.Email);

            if (user == null)
            {
                return new ErrorOrToken { Error = "Invalid email" };
            }

            if (user.PasswordHash == null)
            {
                return new ErrorOrToken { Error = "Invalid password! Try google auth!" };
            }
            var isPassCorrect = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash);

            if (!isPassCorrect)
            {
                return new ErrorOrToken { Error = "Invalid password" };
            }

            var token = _tokenService.CreateToken(user.Id, user.Email, user.Username);
            return new ErrorOrToken { Token = token };
        }

        public async Task<ErrorOrToken> Register(UserRegistrationDto userRegistrationDto)
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

            var token = _tokenService.CreateToken(user.Id, user.Email, user.Username);
            return new ErrorOrToken { Token = token };
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}