using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Dtos.Message
{
    public class CreateMessageDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Message cannot be empty")]
        public string Content { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoomId { get; set; }
    }
}