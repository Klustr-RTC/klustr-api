using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Klustr_api.Dtos.Member
{
    public class UpdateMemberDTO
    {
        [Required(ErrorMessage = "IsAdmin is required")]
        public bool IsAdmin { get; set; }
    }
}