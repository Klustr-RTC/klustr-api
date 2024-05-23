using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Room;
using Klustr_api.Dtos.User;

namespace Klustr_api.Dtos.Message
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public RoomDto? Room { get; set; }
        public UserDto? User { get; set; }
    }
}