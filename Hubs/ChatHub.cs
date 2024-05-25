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
                    await Clients.Group(existingUserRoomConnection.Room).UserLeft(existingUserRoomConnection);
                    await Clients.Group(userRoomConnection.Room).UserJoined(userRoomConnection);
                }
            }
            else
            {
                await Clients.Group(userRoomConnection.Room).UserJoined(userRoomConnection);
                if (noOfUsers >= 1)
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
            await Clients.Group(userRoomConnection.Room).UserLeft(userRoomConnection);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var userRoomConnection))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userRoomConnection.Room);
                _connections.Remove(Context.ConnectionId);
                await Clients.Group(userRoomConnection.Room).UserLeft(userRoomConnection);
                await base.OnDisconnectedAsync(exception);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendConnectedUsers(string room)
        {
            var users = _connections.Values.Where(x => x.Room == room).Select(x => x.User).ToList();
            await Clients.Caller.SendConnectedUsers(users);
        }
    }
}