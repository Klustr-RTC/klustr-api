using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Message;
using Klustr_api.Models;

namespace Klustr_api.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message?> CreateMessageAsync(Message message);
        Task<Message?> GetMessageByIdAsync(string messageId);
        Task<List<Message>> GetMessagesByRoomIdAsync(string roomId);
        Task<bool> DeleteMessageAsync(string messageId, string userId);
        Task<List<Message>> GetMessagesByUserIdAsync(string userId);
    }
}