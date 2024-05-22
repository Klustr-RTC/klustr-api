using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;
using Klustr_api.Models;

namespace Klustr_api.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToUserDtoFromUser(this User userModel)
        {
            return new UserDto
            {
                Id = userModel.Id,
                Avatar = userModel.Avatar,
                Email = userModel.Email,
                Username = userModel.Username
            };
        }
    }
}