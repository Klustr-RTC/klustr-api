using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Properties for Google authentication
        public string GoogleId { get; set; } = string.Empty;
        public string GoogleAccessToken { get; set; } = string.Empty;
        public string GoogleRefreshToken { get; set; } = string.Empty;

        // Navigation properties
        public List<Member> Members { get; set; } = new List<Member>();
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}