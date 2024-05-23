using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Data;
using Klustr_api.Interfaces;
using Klustr_api.Models;
using Microsoft.EntityFrameworkCore;

namespace Klustr_api.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDBContext _context;

        public MessageRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Message?> CreateMessageAsync(Message message)
        {
            var room = await _context.Rooms.FindAsync(message.RoomId);
            if (room == null)
            {
                throw new InvalidOperationException("No room found");
            }
            if (room.SaveMessages)
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
            }
            return message;
        }

        public async Task<Message?> GetMessageByIdAsync(string messageId)
        {
            return await _context.Messages
                .Include(m => m.User)
                .Include(m => m.Room)
                .FirstOrDefaultAsync(m => m.Id.ToString() == messageId);
        }

        public async Task<List<Message>> GetMessagesByRoomIdAsync(string roomId)
        {
            return await _context.Messages
                .Include(m => m.User)
                .Where(m => m.RoomId.ToString() == roomId)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesByUserIdAsync(string userId)
        {
            return await _context.Messages
                .Include(m => m.Room)
                .Where(m => m.UserId.ToString() == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteMessageAsync(string messageId, string userId)
        {
            var message = await GetMessageByIdAsync(messageId);
            if (message == null)
            {
                return false;
            }
            if (message.UserId.ToString() != userId)
            {
                throw new InvalidOperationException("You are not allowed to do this");
            }
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}