using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Models
{
    public class Member
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public bool IsAdmin { get; set; }

        // Navigation properties
        public User? User { get; set; }
    }
}