using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Member;
using Klustr_api.Models;

namespace Klustr_api.Interfaces
{
    public interface IMemberRepository
    {
        Task<Member?> CreateAsync(Member member);
        Task<(bool isSuccess, bool isOwner, bool roomExists)> DeleteAsync(string memberId, string userId);
        Task<Member?> UpdateAsync(string memberId, UpdateMemberDTO member);
        Task<List<Member>> GetMembersByRoomAsync(string roomId);
        Task<List<Member>> GetMembersByUserAsync(string userId);

        Task<Member?> GetMemberByIdAsync(string memberId);
        Task<Member?> GetMemberByUserAndRoomAsync(string roomId, string userId);
    }
}