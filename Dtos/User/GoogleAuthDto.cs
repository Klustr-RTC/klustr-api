using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Dtos.User
{
    public class GoogleAuthDto
    {

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Access token is required")]
        public string GoogleAccessToken { get; set; } = string.Empty;

    }
    public class GoogleUserInfo
    {
        public string Sub { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailVerified { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public string GivenName { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
    }
    public class ErrorOrToken
    {
        public string? Error { get; set; }
        public string? Token { get; set; }
    }
}