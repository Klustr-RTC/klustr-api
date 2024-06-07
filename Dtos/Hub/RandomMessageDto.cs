using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;

namespace Klustr_api.Dtos.Hub
{
    public class RandomMessageDto
    {
        public required string content { get; set; }
        public required UserDto user { get; set; }

        public DateTime? timeStamp { get; set; }
    }
}