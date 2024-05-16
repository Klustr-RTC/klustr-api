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
        Task<User?> FindByUsername(string username);
    }
}