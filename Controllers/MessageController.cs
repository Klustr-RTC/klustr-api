using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Message;
using Klustr_api.Hubs;
using Klustr_api.Hubs.Clients;
using Klustr_api.Interfaces;
using Klustr_api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Klustr_api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public MessageController(IMessageRepository messageRepo, IHubContext<ChatHub, IChatClient> hubContext, IUserRepository userRepo)
        {
            _messageRepo = messageRepo;
            _hubContext = hubContext;
            _userRepo = userRepo;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDto createMessageDto)
        {
            try
            {
                var messageDto = new MessageDto
                {
                    Id = Guid.NewGuid(),
                    Content = createMessageDto.Content,
                    Timestamp = DateTime.UtcNow,
                    UserId = createMessageDto.UserId,
                    RoomId = createMessageDto.RoomId,
                };
                var user = await _userRepo.FindById(createMessageDto.UserId.ToString());
                if (user == null)
                {
                    return NotFound("Sender not found.");
                }
                messageDto.User = user.ToUserDtoFromUser();
                await _hubContext.Clients.Group(createMessageDto.RoomId.ToString()).ReceiveMessage(messageDto);
                var message = messageDto.ToMessageFromMessageDto();
                var result = await _messageRepo.CreateMessageAsync(message);
                if (result == null)
                {
                    return BadRequest("Message could not be saved.");
                }

                return Ok(messageDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetMessageById/{messageId}")]
        public async Task<IActionResult> GetMessageById([FromRoute] string messageId)
        {
            if (!Guid.TryParse(messageId, out Guid messageGuid))
            {
                return BadRequest("Invalid message ID format.");
            }
            var message = await _messageRepo.GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return NotFound();
            }

            var messageDto = message.ToMessageDtoFromMessage();
            return Ok(messageDto);
        }

        [HttpGet("GetMessagesByRoomId/{roomId}")]
        public async Task<IActionResult> GetMessagesByRoomId([FromRoute] string roomId)
        {
            if (!Guid.TryParse(roomId, out Guid roomGuid))
            {
                return BadRequest("Invalid room ID format.");
            }
            var messages = await _messageRepo.GetMessagesByRoomIdAsync(roomId);
            var messageDtos = messages.Select(m => m.ToMessageDtoFromMessage());
            return Ok(messageDtos);
        }

        [HttpGet("GetMessagesByUserId/{userId}")]
        public async Task<IActionResult> GetMessagesByUserId([FromRoute] string userId)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("Invalid user ID format.");
            }
            var messages = await _messageRepo.GetMessagesByUserIdAsync(userId);
            var messageDtos = messages.Select(m => m.ToMessageDtoFromMessage());
            return Ok(messageDtos);
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] string messageId)
        {
            try
            {
                if (!Guid.TryParse(messageId, out Guid messageGuid))
                {
                    return BadRequest("Invalid message ID format.");
                }
                var userId = User.FindFirst("userId")?.Value!;
                var result = await _messageRepo.DeleteMessageAsync(messageId, userId);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}