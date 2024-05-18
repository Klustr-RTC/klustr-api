using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Dtos.Member
{
    public class CreateMemberDTO
    {
        [Required(ErrorMessage = "RoomId is required")]
        public required string RoomId { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public required string UserId { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}