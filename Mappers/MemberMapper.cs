using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Member;
using Klustr_api.Models;

namespace Klustr_api.Mappers
{
    public static class MemberMapper
    {
        public static Member ToMemberFromCreateDTO(this CreateMemberDTO createMemberDto)
        {
            return new Member
            {
                RoomId = Guid.Parse(createMemberDto.RoomId),
                UserId = Guid.Parse(createMemberDto.UserId),
                IsAdmin = createMemberDto.IsAdmin
            };
        }
    }
}