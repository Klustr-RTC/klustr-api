using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Member;
using Klustr_api.Interfaces;
using Klustr_api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Klustr_api.Controllers
{
    [ApiController]
    [Route("api/member")]
    public class MemberController(IMemberRepository memberRepo, IRoomRepository roomRepo) : ControllerBase
    {
        private readonly IMemberRepository _memberRepo = memberRepo;
        private readonly IRoomRepository _roomRepo = roomRepo;

        [HttpPost()]
        public async Task<IActionResult> CreateMember([FromBody] CreateMemberDTO createMemberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var memberModel = createMemberDto.ToMemberFromCreateDTO();
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var room = await _roomRepo.GetRoomByIdAsync(memberModel.RoomId.ToString());
                if (room == null)
                {
                    return NotFound("Room not found");
                }
                if (room.CreatedBy != userId)
                {
                    return Unauthorized("You are not the owner of the room");
                }
                var member = await _memberRepo.CreateAsync(memberModel);
                return Ok(member);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{memberId}")]
        public async Task<IActionResult> DeleteMember(string memberId)
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var (isSuccess, isOwner, roomExists) = await _memberRepo.DeleteAsync(memberId, userId!);
                if (!isSuccess)
                {
                    return NotFound("Member not found");
                }
                if (!isOwner)
                {
                    return Unauthorized("You are not Allowed to delete this member");
                }
                if (!roomExists)
                {
                    return NotFound("Room not found");
                }
                return Ok("Member deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{memberId}")]
        public async Task<IActionResult> UpdateMember(string memberId, [FromBody] UpdateMemberDTO updateMemberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Guid.TryParse(memberId, out Guid memberGuid))
            {
                return BadRequest("Invalid member ID format.");
            }
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var member = await _memberRepo.GetMemberByIdAsync(memberId);
                if (member == null)
                {
                    return NotFound("Member not found");
                }
                var room = await _roomRepo.GetRoomByIdAsync(member.RoomId.ToString());
                if (room == null)
                {
                    return NotFound("Room not found");
                }
                if (room.CreatedBy != userId)
                {
                    return Unauthorized("You are not the owner of the room");
                }
                var updatedMember = await _memberRepo.UpdateAsync(memberId, updateMemberDto);
                return Ok(updatedMember);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetMembersByRoom/{roomId}")]
        public async Task<IActionResult> GetMembersByRoom(string roomId)
        {
            if (!Guid.TryParse(roomId, out Guid roomGuid))
            {
                return BadRequest("Invalid room ID format.");
            }
            try
            {
                var isMember = await _memberRepo.GetMemberByUserAndRoomAsync(roomId, User.FindFirst("userId")?.Value!);
                if (isMember == null)
                {
                    return Unauthorized("You are not a member of this room");
                }
                var members = await _memberRepo.GetMembersByRoomAsync(roomId);
                return Ok(members);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}