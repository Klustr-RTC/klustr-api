using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using Klustr_api.Data;
using Klustr_api.Dtos.User;
using Klustr_api.Interfaces;
using Klustr_api.Mappers;
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

        public async Task<User?> FindById(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
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
                return new ErrorOrToken { Error = "Invalid Credentials" };
            }

            if (user.PasswordHash == null)
            {
                return new ErrorOrToken { Error = "Invalid Credentials! Try google auth!" };
            }
            var isPassCorrect = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.PasswordHash);

            if (!isPassCorrect)
            {
                return new ErrorOrToken { Error = "Invalid Credentials" };
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

        public async Task<bool> DeleteAccountAsync(string userId)
        {
            var user = await FindById(userId);
            if (user == null)
            {
                return false;
            }

            var members = _context.Members.Where(m => m.UserId.ToString() == userId);
            _context.Members.RemoveRange(members);

            var messages = _context.Messages.Where(m => m.UserId.ToString() == userId);
            _context.Messages.RemoveRange(messages);

            var rooms = _context.Rooms.Where(r => r.CreatedBy == userId);
            _context.Rooms.RemoveRange(rooms);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User?> UpdateUserAsync(string userId, UpdateUserDto updateUserDto)
        {
            var user = await FindById(userId);
            if (user == null)
            {
                return null;
            }

            // Check if email is unique
            if (updateUserDto.Email != user.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email);
                if (emailExists)
                {
                    throw new InvalidOperationException("Email is already taken.");
                }
            }

            // Check if username is unique
            if (updateUserDto.Username != user.Username)
            {
                var usernameExists = await _context.Users.AnyAsync(u => u.Username == updateUserDto.Username);
                if (usernameExists)
                {
                    throw new InvalidOperationException("Username is already taken.");
                }
            }

            user.Avatar = updateUserDto.Avatar;
            user.Email = updateUserDto.Email;
            user.Username = updateUserDto.Username;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<UserDto>> FindUsers(string query)
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => u.ToUserDtoFromUser()).Where(u => u.Username.Contains(query, StringComparison.CurrentCultureIgnoreCase) || u.Email.Contains(query, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
    }
}