using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klustr_api.Dtos.Message;
using Klustr_api.Models;

namespace Klustr_api.Hubs.Clients
{
    public interface IChatClient
    {
        Task ReceiveMessage(MessageDto messageDto);
    }
}