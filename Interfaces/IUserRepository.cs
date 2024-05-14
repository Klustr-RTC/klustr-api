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
        Task<string?> Register(UserRegistrationDto userRegistrationDto);
        Task<string?> Login(UserLoginDto userLoginDto);
        Task<string?> GoogleAuth(GoogleAuthDto googleAuthDto);
        Task<bool> UserExists(string email);
        Task<User?> FindByEmail(string email);
    }
}