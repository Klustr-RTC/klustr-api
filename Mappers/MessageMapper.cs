using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Message;
using Klustr_api.Models;

namespace Klustr_api.Mappers
{
    public static class MessageMapper
    {
        public static MessageDto ToMessageDtoFromMessage(this Message messageModel)
        {
            return new MessageDto
            {
                Content = messageModel.Content,
                Id = messageModel.Id,
                RoomId = messageModel.RoomId,
                Timestamp = messageModel.Timestamp,
                UserId = messageModel.UserId,
                Room = messageModel?.Room?.ToRoomDtoFromRoom(),
                User = messageModel?.User?.ToUserDtoFromUser()
            };
        }
        public static Message ToMessageFromMessageDto(this MessageDto messageDto)
        {
            return new Message
            {
                Content = messageDto.Content,
                Id = messageDto.Id,
                RoomId = messageDto.RoomId,
                Timestamp = messageDto.Timestamp,
                UserId = messageDto.UserId,
            };
        }
    }
}