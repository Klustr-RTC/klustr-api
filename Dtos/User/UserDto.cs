using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Dtos.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}