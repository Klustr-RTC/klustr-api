using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Hub;
using Klustr_api.Dtos.Message;
using Klustr_api.Dtos.User;
using Klustr_api.Models;

namespace Klustr_api.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(MessageDto messageDto);
        Task UserJoined(UserRoomConnection userRoomConnection);
        Task UserLeft(UserRoomConnection userRoomConnection);
        Task SendConnectedUsers(List<UserDto?> users);
        Task JoinRoomResponse(int result, int count);
    }
}