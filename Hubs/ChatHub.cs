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
    public class ChatHub(List<string> waitingUsers, IDictionary<string, UserRoomConnection> connections, IDictionary<string, string?> randomPairs, IDictionary<string, UserDto> randomUsers) : Hub<IChatClient>
    {
        private readonly IDictionary<string, UserRoomConnection> _connections = connections;
        private readonly IDictionary<string, string?> _randomPairs = randomPairs;
        private readonly IDictionary<string, UserDto> _randomUsers = randomUsers;
        private List<string> _waitingUsers = waitingUsers;
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
        public override Task OnConnectedAsync()
        {
            Clients.Caller.OnCount(_randomUsers.Count);
            return base.OnConnectedAsync();
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
            await LeaveRandomRoom();
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

        public async Task JoinRandomRoom(UserDto user, VideoConfig config, string peerId)
        {
            if (_randomPairs.ContainsKey(Context.ConnectionId))
            {
                return;
            }
            _randomUsers.Add(Context.ConnectionId, user);
            _randomPairs.Add(Context.ConnectionId, null);
            await Clients.All.OnCount(_randomUsers.Count);
            if (_waitingUsers.Count > 0)
            {
                await ConnectRandomUser(Context.ConnectionId, config, peerId);
            }
            else
            {
                if (!_waitingUsers.Contains(Context.ConnectionId))
                {
                    _waitingUsers.Add(Context.ConnectionId);
                }
            }
            await Clients.Client(Context.ConnectionId).JoinRoomResponse(1, 0);
        }

        public async Task LeaveRandomRoom()
        {
            if (_randomPairs.TryGetValue(Context.ConnectionId, out var randomUser))
            {
                if (randomUser != null)
                {
                    if (_randomPairs.ContainsKey(randomUser))
                    {
                        _randomPairs[randomUser] = null;
                    }
                    await Clients.Client(randomUser).SkipUser();
                }
            }
            _randomPairs.Remove(Context.ConnectionId);
            _randomUsers.Remove(Context.ConnectionId);
            _waitingUsers.RemoveAll((u) => u == Context.ConnectionId);
            await Clients.AllExcept(Context.ConnectionId).OnCount(_randomUsers.Count);
        }
        public async Task SkipUser(VideoConfig config, string peerId)
        {
            if (_randomPairs.TryGetValue(Context.ConnectionId, out var randomUser))
            {
                _randomPairs[Context.ConnectionId] = null;
                if (randomUser != null)
                {
                    if (_randomPairs.ContainsKey(randomUser))
                    {
                        _randomPairs[randomUser] = null;
                    }
                    await Clients.Client(randomUser).SkipUser();
                }
                if (_waitingUsers.Count > 0)
                {
                    await ConnectRandomUser(Context.ConnectionId, config, peerId);
                }
                else
                {
                    if (!_waitingUsers.Contains(Context.ConnectionId))
                    {
                        _waitingUsers.Add(Context.ConnectionId);
                    }
                }
            }
        }
        public async Task ToggleRandomVideo(string peerId, bool isVideoOn)
        {
            if (_randomPairs.TryGetValue(Context.ConnectionId, out var id))
            {
                if (id != null)
                    await Clients.Client(id).ToggleVideo(peerId, isVideoOn);
            }
        }
        public async Task ToggleRandomAudio(string peerId, bool isAudioOn)
        {
            if (_randomPairs.TryGetValue(Context.ConnectionId, out var id))
            {
                if (id != null)
                    await Clients.Client(id).ToggleAudio(peerId, isAudioOn);
            }
        }
        public async Task ConnectRandomUser(string id, VideoConfig config, string peerId)
        {
            var user = _randomUsers.TryGetValue(id, out var value) ? value : null;
            if (user == null)
            {
                return;
            }
            var randomUser = _waitingUsers.First();
            _waitingUsers.RemoveAt(0);
            while (!_randomUsers.ContainsKey(randomUser) || randomUser == id)
            {
                if (_waitingUsers.Count == 0)
                {
                    break;
                }
                randomUser = _waitingUsers.First();
                _waitingUsers.RemoveAt(0);
            }
            if (!_randomUsers.ContainsKey(randomUser))
            {
                return;
            }
            Console.WriteLine($"Connecting {user.Username} to {_randomUsers[randomUser].Username}");
            _randomPairs[id] = randomUser;
            _randomPairs[randomUser] = id;
            await Clients.Client(randomUser).RandomUserJoined(user, config, peerId);
        }
        public async Task SendRandomMessage(RandomMessageDto messageDto)
        {
            Console.WriteLine("SendRandomMessage", messageDto.content);
            if (_randomPairs.TryGetValue(Context.ConnectionId, out var randomUser))
            {
                if (randomUser != null)
                {
                    messageDto.timeStamp = DateTime.Now;
                    await Clients.Client(randomUser).ReceiveRandomMessage(messageDto);
                }
            }
        }
    }
}