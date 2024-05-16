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
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepo;

        public RoomController(IRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto createRoomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var roomModel = createRoomDto.ToRoomFromCreateDTO();
            var userId = User.FindFirst("userId")?.Value;
            roomModel.CreatedBy = userId!;
            var room = await _roomRepo.CreateAsync(roomModel);
            if (room == null)
            {
                return StatusCode(500, "Failed to create room.");
            }
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
            var result = await _roomRepo.DeleteAsync(roomId, userId!);
            if (!result.isSuccess)
            {
                if (!result.roomExists)
                {
                    return NoContent();
                }
                if (!result.isOwner)
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

    }
}