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

        public async Task<bool> DeleteAsync(string roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return false;
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Room?> UpdateAsync(string roomId, UpdateRoomDto updateRoomDto)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return null;
            }

            // Update properties if provided
            if (updateRoomDto.Name != null)
            {
                room.Name = updateRoomDto.Name;
            }

            if (updateRoomDto.Description != null)
            {
                room.Description = updateRoomDto.Description;
            }

            if (updateRoomDto.Type.HasValue)
            {
                room.Type = updateRoomDto.Type.Value;
            }

            if (updateRoomDto.SaveMessages.HasValue)
            {
                room.SaveMessages = updateRoomDto.SaveMessages.Value;
            }

            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<List<Room>> GetRoomsAsync(QueryObject query)
        {
            var rooms = _context.Rooms.AsQueryable();

            // Apply filtering, sorting, paging, etc. based on query parameters

            return await rooms.ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(string roomId)
        {
            return await _context.Rooms.FindAsync(roomId);
        }

        public async Task<Room?> GetRoomByJoinCodeAsync(string joinCode)
        {
            return await _context.Rooms.SingleOrDefaultAsync(r => r.JoinCode == joinCode);
        }
    }
}