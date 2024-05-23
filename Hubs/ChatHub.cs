using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Message;
using Klustr_api.Hubs.Clients;
using Klustr_api.Models;
using Microsoft.AspNetCore.SignalR;

namespace Klustr_api.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
    }
}