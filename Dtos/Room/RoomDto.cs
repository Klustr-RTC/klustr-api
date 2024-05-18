using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Dtos.Room
{
    public class RoomDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsPublic { get; set; } = true;

        public string Description { get; set; } = string.Empty;

        public Models.Room.RoomType Type { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ShareableLink { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool SaveMessages { get; set; } = false;
    }
}