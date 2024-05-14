using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Dtos.User
{
    public class GoogleAuthDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Google ID is required")]
        public string GoogleId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Access token is required")]
        public string GoogleAccessToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Refresh token is required")]
        public string GoogleRefreshToken { get; set; } = string.Empty;

    }
}