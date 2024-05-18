using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Klustr_api.Dtos;
using Klustr_api.Dtos.Room;
using Klustr_api.Helpers;
using Klustr_api.Interfaces;
using Klustr_api.Mappers;
using Klustr_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Klustr_api.Controllers
{
    [ApiController]
    [Route("api/room")]
    [Authorize]
    public class RoomController(IRoomRepository roomRepo, IMemberRepository memberRepo) : ControllerBase
    {
        private readonly IRoomRepository _roomRepo = roomRepo;
        private readonly IMemberRepository _memberRepo = memberRepo;

        [HttpPost()]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto createRoomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var roomModel = createRoomDto.ToRoomFromCreateDTO();
            var userId = User.FindFirst("userId")?.Value;
            // assigning user as the owner of the room
            roomModel.CreatedBy = userId!;
            // if the room is audio video type, save messages will be false
            if (roomModel.Type == Room.RoomType.AudioVideo)
            {
                roomModel.SaveMessages = false;
            }
            var room = await _roomRepo.CreateAsync(roomModel);
            if (room == null)
            {
                return StatusCode(500, "Failed to create room.");
            }
            // add the owner as a member of the room
            var member = new Member
            {
                RoomId = room.Id,
                UserId = Guid.Parse(userId!),
                IsAdmin = true
            };
            await _memberRepo.CreateAsync(member);
            var roomDto = room.ToRoomDtoFromRoom();
            var result = new { Room = roomDto, JoinCode = room.JoinCode };

            return Ok(result);
        }

        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoom([FromRoute] string roomId)
        {

            if (!Guid.TryParse(roomId, out Guid roomGuid))
            {
                return BadRequest("Invalid room ID format.");
            }

            var userId = User.FindFirst("userId")?.Value;
            var (isSuccess, isOwner, roomExists) = await _roomRepo.DeleteAsync(roomId, userId!);
            if (!isSuccess)
            {
                if (!roomExists)
                {
                    return NotFound("Room not found.");
                }
                if (!isOwner)
                {
                    return StatusCode(403, "You are not allowed to delete this room.");
                }
                return StatusCode(500, "Error deleting the room.");
            }
            return Ok("Room deleted successfully.");
        }

        [HttpGet("GetRoomByJoinCode/{joinCode}")]
        public async Task<IActionResult> GetRoomByCode([FromRoute] string joinCode)
        {
            if (!int.TryParse(joinCode, out int joinCodeInt))
            {
                return BadRequest("Invalid join code");
            }
            var room = await _roomRepo.GetRoomByJoinCodeAsync(joinCode);

            if (room == null)
            {
                return NotFound("Room not found");
            }

            return Ok(room.ToRoomDtoFromRoom());
        }

        [HttpGet("GetRoomById/{roomId}")]
        public async Task<IActionResult> GetRoomById([FromRoute] string roomId)
        {
            if (!Guid.TryParse(roomId, out Guid roomGuid))
            {
                return BadRequest("Invalid room ID format.");
            }
            var room = await _roomRepo.GetRoomByIdAsync(roomId);

            if (room == null)
            {
                return NotFound("Room not found");
            }

            return Ok(room.ToRoomDtoFromRoom());
        }

        [HttpGet("GetAllRooms")]
        public async Task<IActionResult> GetAllRooms([FromQuery] QueryObject queryObject)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var rooms = await _roomRepo.GetRoomsAsync(queryObject);
            var roomDto = rooms.Select(r => r?.ToRoomDtoFromRoom()).ToList();
            return Ok(roomDto);
        }

        [HttpPut("{roomId}")]
        public async Task<IActionResult> UpdateRoom([FromRoute] string roomId, [FromBody] UpdateRoomDto updateRoomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Guid.TryParse(roomId, out Guid roomGuid))
            {
                return BadRequest("Invalid room ID format.");
            }

            var userId = User.FindFirst("userId")?.Value;
            var result = await _roomRepo.UpdateAsync(roomId, userId!, updateRoomDto);
            if (result == null)
            {
                return StatusCode(500, "Failed to update room");
            }
            return Ok(result.ToRoomDtoFromRoom());
        }
        [HttpPost("{roomId}/generate-link")]
        public async Task<IActionResult> GenerateNewShareableLink(string roomId)
        {
            var userId = User.FindFirst("userId")?.Value;
            try
            {
                var newLink = await _roomRepo.GenerateNewShareableLinkAsync(roomId, userId!);
                return Ok(new { shareableLink = newLink });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You are not authorized to generate Link for this room.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{roomId}/verifyJoinCode/{joinCode}")]
        public async Task<IActionResult> VerifyJoinCode(string roomId, string joinCode)
        {
            var room = await _roomRepo.GetRoomByIdAsync(roomId);
            if (room == null)
            {
                return NotFound("Room not found");
            }
            if (room.JoinCode != joinCode)
            {
                return BadRequest("Invalid join code");
            }
            return Ok(room.ToRoomDtoFromRoom());
        }
        [HttpGet("{roomId}/GetJoinCode")]
        public async Task<IActionResult> GetJoinCode(string roomId)
        {
            var userId = User.FindFirst("userId")?.Value;
            var room = await _roomRepo.GetRoomByIdAsync(roomId);
            if (room == null)
            {
                return NotFound("Room not found");
            }
            var member = await _memberRepo.GetMemberByUserAndRoomAsync(roomId, userId!);
            if (member == null)
            {
                return Unauthorized("You are not a member of this room");
            }
            if (!member.IsAdmin)
            {
                return Unauthorized("You are not authorized to get join code");
            }
            return Ok(new { joinCode = room.JoinCode });
        }
    }
}