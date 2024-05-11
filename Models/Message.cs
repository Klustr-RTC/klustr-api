using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RoomId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Room? Room { get; set; }
    }
}