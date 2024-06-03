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
        Task DeleteMessage(string messageId);
        Task UserJoined(UserRoomConnection userRoomConnection, string id);
        Task UserLeft(UserRoomConnection userRoomConnection, string id);
        Task SendConnectedUsers(List<UserDto?> users);
        Task JoinRoomResponse(int result, int count);
        Task NewPeer(string id, UserDto user, VideoConfig config);
        Task ToggleVideo(string id, bool isVideoOn);
        Task ToggleAudio(string id, bool isAudioOn);
    }
}