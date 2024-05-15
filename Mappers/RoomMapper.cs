using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos;
using Klustr_api.Dtos.Room;
using Klustr_api.Models;

namespace Klustr_api.Mappers
{
    public static class RoomMapper
    {
        public static Room ToRoomFromCreateDTO(this CreateRoomDto createRoomDto)
        {
            return new Room
            {
                Description = createRoomDto.Description,
                IsPublic = createRoomDto.IsPublic,
                SaveMessages = createRoomDto.SaveMessages,
                Name = createRoomDto.Name,
                Type = createRoomDto.Type
            };
        }

        public static RoomDto ToRoomDtoFromRoom(this Room roomModel)
        {
            return new RoomDto
            {
                Id = roomModel.Id,
                Description = roomModel.Description,
                IsPublic = roomModel.IsPublic,
                SaveMessages = roomModel.SaveMessages,
                Name = roomModel.Name,
                CreatedBy = roomModel.CreatedBy,
                CreatedOn = roomModel.CreatedOn,
                Type = roomModel.Type,
            };
        }
    }
}