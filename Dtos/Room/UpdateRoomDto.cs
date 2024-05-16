using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Dtos.Room
{
    public class UpdateRoomDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string? Description { get; set; }

        [Required]
        public Models.Room.RoomType? Type { get; set; }

        [Required]
        public bool? SaveMessages { get; set; }

        [Required]
        public bool? IsPublic { get; set; }
    }
}