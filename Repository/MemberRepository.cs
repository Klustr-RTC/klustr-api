using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Data;
using Klustr_api.Dtos.Member;
using Klustr_api.Interfaces;
using Klustr_api.Models;
using Microsoft.EntityFrameworkCore;

namespace Klustr_api.Repository
{
    public class MemberRepository(ApplicationDBContext context) : IMemberRepository
    {
        private readonly ApplicationDBContext _context = context;

        public async Task<Member?> CreateAsync(Member member)
        {
            var room = await _context.Rooms
                        .Include(r => r.Members)
                        .FirstOrDefaultAsync(r => r.Id.ToString() == member.RoomId.ToString()) ?? throw new Exception("Room not found");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == member.UserId.ToString()) ?? throw new Exception("User not found");

            if (room.Members.Any(m => m.UserId == user.Id))
            {
                throw new Exception("Member already in the room");
            }

            var Member = await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
            return Member.Entity;
        }

        public async Task<(bool isSuccess, bool isOwner, bool roomExists)> DeleteAsync(string memberId, string userId)
        {
            try
            {
                var member = await GetMemberByIdAsync(memberId);
                if (member == null)
                {
                    return (false, false, false);
                }
                // user is not itself to delete member
                if (member.UserId.ToString() != userId)
                {
                    var requestMember = await GetMemberByUserAndRoomAsync(member.RoomId.ToString(), userId);
                    // user is not admin of the room
                    if (requestMember == null || requestMember.IsAdmin == false)
                    {
                        return (false, false, true);
                    }
                }
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
                return (true, true, true);
            }
            catch (Exception)
            {
                return (false, true, true);
            }
        }

        public async Task<Member?> GetMemberByIdAsync(string memberId)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.Id.ToString() == memberId);
            return member;
        }

        public async Task<Member?> GetMemberByUserAndRoomAsync(string roomId, string userId)
        {
            var member = await _context.Members.FirstOrDefaultAsync(m => m.RoomId.ToString() == roomId && m.UserId.ToString() == userId);
            return member;
        }

        public async Task<List<Member>> GetMembersByRoomAsync(string roomId)
        {
            var members = await _context.Members.Where(m => m.RoomId.ToString() == roomId).ToListAsync();
            return members;
        }

        public async Task<List<Member>> GetMembersByUserAsync(string userId)
        {
            var members = await _context.Members.Where(m => m.UserId.ToString() == userId).ToListAsync();
            return members;
        }

        public async Task<Member?> UpdateAsync(string memberId, UpdateMemberDTO member)
        {
            var existingMember = await GetMemberByIdAsync(memberId) ?? throw new Exception("Member not found");
            existingMember.IsAdmin = member.IsAdmin;
            await _context.SaveChangesAsync();
            return existingMember;
        }
    }
}