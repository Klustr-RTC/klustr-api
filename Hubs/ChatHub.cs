using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Hub;
using Klustr_api.Dtos.Message;
using Klustr_api.Dtos.User;
using Klustr_api.Hubs.Clients;
using Klustr_api.Models;
using Microsoft.AspNetCore.SignalR;

namespace Klustr_api.Hubs
{
    public class ChatHub(IDictionary<string, UserRoomConnection> connections) : Hub<IChatClient>
    {
        private readonly IDictionary<string, UserRoomConnection> _connections = connections;

        public async Task JoinRoom(UserRoomConnection userRoomConnection)
        {
            var noOfUsers = _connections.Values.Count(x => x.Room == userRoomConnection.Room);
            if (_connections.TryGetValue(Context.ConnectionId, out var existingUserRoomConnection))
            {
                if (existingUserRoomConnection.Room != userRoomConnection.Room)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, existingUserRoomConnection.Room);
                    await Clients.Group(existingUserRoomConnection.Room).UserLeft(existingUserRoomConnection, Context.ConnectionId);
                    await Clients.Group(userRoomConnection.Room).UserJoined(userRoomConnection, Context.ConnectionId);
                }
            }
            else
            {
                await Clients.Group(userRoomConnection.Room).UserJoined(userRoomConnection, Context.ConnectionId);
                if (noOfUsers >= 16)
                {
                    await Clients.Caller.JoinRoomResponse(2, noOfUsers);
                    return;
                }
            }
            await Clients.Caller.JoinRoomResponse(1, noOfUsers);
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoomConnection.Room);
            _connections[Context.ConnectionId] = userRoomConnection;
            await SendConnectedUsers(userRoomConnection.Room);
        }

        public async Task LeftRoom(UserRoomConnection userRoomConnection)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRoomConnection.Room);
            _connections.Remove(Context.ConnectionId);
            await Clients.Group(userRoomConnection.Room).UserLeft(userRoomConnection, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var userRoomConnection))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRoomConnection.Room);
                _connections.Remove(Context.ConnectionId);
                await Clients.Group(userRoomConnection.Room).UserLeft(userRoomConnection, Context.ConnectionId);

                await base.OnDisconnectedAsync(exception);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendConnectedUsers(string room)
        {
            var users = _connections.Values.Where(x => x.Room == room).Select(x => x.User).ToList();
            await Clients.Caller.SendConnectedUsers(users);
        }

        // for video call
        public async Task JoinVideoRoom(UserRoomConnection userRoomConnection, string id, VideoConfig config)
        {
            var noOfUsers = _connections.Values.Count(x => x.Room == userRoomConnection.Room);
            if (_connections.TryGetValue(Context.ConnectionId, out var existingUserRoomConnection))
            {
                if (existingUserRoomConnection.Room != userRoomConnection.Room)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, existingUserRoomConnection.Room);
                    await Clients.Group(existingUserRoomConnection.Room).UserLeft(existingUserRoomConnection, Context.ConnectionId);
                    await Clients.Group(userRoomConnection.Room).UserJoined(userRoomConnection, Context.ConnectionId);
                }
            }
            else
            {
                await Clients.Group(userRoomConnection.Room).UserJoined(userRoomConnection, Context.ConnectionId);
                if (noOfUsers >= 16)
                {
                    await Clients.Caller.JoinRoomResponse(2, noOfUsers);
                    return;
                }
            }
            await Clients.Caller.JoinRoomResponse(1, noOfUsers);
            await Groups.AddToGroupAsync(Context.ConnectionId, userRoomConnection.Room);
            _connections[Context.ConnectionId] = userRoomConnection;
            await SendConnectedUsers(userRoomConnection.Room);
            await Clients.Group(userRoomConnection.Room).NewPeer(id, userRoomConnection.User!, config);
        }
        public async Task ToggleVideo(string peerId, bool isVideoOn)
        {
            await Clients.Group(_connections[Context.ConnectionId].Room).ToggleVideo(peerId, isVideoOn);
        }
        public async Task ToggleAudio(string peerId, bool isAudioOn)
        {
            await Clients.Group(_connections[Context.ConnectionId].Room).ToggleAudio(peerId, isAudioOn);
        }
    }
}