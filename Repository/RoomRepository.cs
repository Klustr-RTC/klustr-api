using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Data;
using Klustr_api.Dtos;
using Klustr_api.Dtos.Room;
using Klustr_api.Helpers;
using Klustr_api.Interfaces;
using Klustr_api.Models;
using Microsoft.EntityFrameworkCore;

namespace Klustr_api.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDBContext _context;

        public RoomRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Room?> CreateAsync(Room roomModel)
        {
            await _context.Rooms.AddAsync(roomModel);
            await _context.SaveChangesAsync();
            return roomModel;
        }

        public async Task<(bool isSuccess, bool isOwner, bool roomExists)> DeleteAsync(string roomId, string userId)
        {
            try
            {
                var room = await GetRoomByIdAsync(roomId);
                if (room == null)
                {
                    return (false, false, false);
                }
                if (room.CreatedBy != userId)
                {
                    return (false, false, true);
                }

                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return (true, true, true);
            }
            catch (Exception e)
            {
                return (false, true, true);
            }
        }

        public async Task<Room?> UpdateAsync(string roomId, string userId, UpdateRoomDto updateRoomDto)
        {
            try
            {
                var existingRoom = await GetRoomByIdAsync(roomId);
                if (existingRoom == null)
                {
                    return null;
                }
                if (existingRoom.CreatedBy != userId)
                {
                    return null;
                }
                existingRoom.Name = updateRoomDto?.Name!;
                existingRoom.Description = updateRoomDto?.Description!;
                existingRoom.Type = updateRoomDto?.Type ?? 0;
                existingRoom.SaveMessages = updateRoomDto?.SaveMessages ?? false;
                existingRoom.IsPublic = updateRoomDto?.IsPublic ?? false;

                await _context.SaveChangesAsync();
                return existingRoom;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<Room>> GetRoomsAsync(QueryObject query)
        {
            var rooms = _context.Rooms.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                rooms = rooms.Where(r => r.Name.Contains(query.Name));
            }
            if (!string.IsNullOrWhiteSpace(query.Description))
            {
                rooms = rooms.Where(r => r.Description.Contains(query.Description));
            }
            if (query.Type.HasValue)
            {
                rooms = rooms.Where(r => r.Type == query.Type.Value);
            }
            if (query.isPublic)
            {
                rooms = rooms.Where(r => r.IsPublic);
            }
            if (query.MinMembers.HasValue)
            {
                rooms = rooms.Where(r => r.Members.Count >= query.MinMembers.Value);
            }

            if (query.MaxMembers.HasValue)
            {
                rooms = rooms.Where(r => r.Members.Count <= query.MaxMembers.Value);
            }
            return await rooms.ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(string roomId)
        {
            return await _context.Rooms.FirstOrDefaultAsync(r => r.Id.ToString() == roomId);
        }

        public async Task<Room?> GetRoomByJoinCodeAsync(string joinCode)
        {
            return await _context.Rooms.SingleOrDefaultAsync(r => r.JoinCode == joinCode);
        }
    }
}