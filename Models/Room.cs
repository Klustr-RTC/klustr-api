using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Models
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsPublic { get; set; } = true;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        public enum RoomType { ChatOnly, AudioVideo }

        [Required]
        public RoomType Type { get; set; }
        public bool SaveMessages { get; set; } = false;
        public string JoinCode { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        // Navigation properties
        public List<Member> Members { get; set; } = new List<Member>();
        public List<Message> Messages { get; set; } = new List<Message>();

        public Room()
        {
            Id = Guid.NewGuid();
            JoinCode = GenerateJoinCode();
        }

        private string GenerateJoinCode()
        {
            var random = new Random();
            return random.Next(10000, 100000).ToString();
        }
    }
}