using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;
using Klustr_api.Models;

namespace Klustr_api.Interfaces
{
    public interface IUserRepository
    {
        Task<ErrorOrToken> Register(UserRegistrationDto userRegistrationDto);
        Task<ErrorOrToken> Login(UserLoginDto userLoginDto);
        Task<ErrorOrToken> GoogleAuth(GoogleAuthDto googleAuthDto);
        Task<bool> UserExists(string email);
        Task<User?> FindByEmail(string email);
        Task<User?> FindById(string userId);
        Task<User?> FindByUsername(string username);
        Task<List<UserDto>> FindUsers(string query);
        Task<User?> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteAccountAsync(string userId);
    }
}