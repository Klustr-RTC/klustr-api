using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos;
using Klustr_api.Dtos.Room;
using Klustr_api.Helpers;
using Klustr_api.Models;

namespace Klustr_api.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room?> CreateAsync(Room roomModel);
        Task<bool> DeleteAsync(string roomId);
        Task<Room?> UpdateAsync(string roomId, UpdateRoomDto updateRoomDto);
        Task<List<Room?>> GetRoomsAsync(QueryObject query);
        Task<Room?> GetRoomByIdAsync(string roomId);
        Task<Room?> GetRoomByJoinCodeAsync(string joinCode);
    }
}