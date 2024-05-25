using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;

namespace Klustr_api.Dtos.Hub
{
    public class UserRoomConnection
    {
        public UserDto? User { get; set; }
        public required string Room { get; set; }
    }
}