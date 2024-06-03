using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Helpers
{
    public class QueryObject
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Models.Room.RoomType? Type { get; set; }
        public bool? isPublic { get; set; }
    }
}