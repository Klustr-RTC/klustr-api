using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Klustr_api.Dtos;
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
    }
}