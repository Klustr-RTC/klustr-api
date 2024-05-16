using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;

namespace Klustr_api.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(Guid userId, string email, string username);
        public Task<GoogleUserInfo?> GetUserInfoFromAccessToken(string accessToken);
    }
}