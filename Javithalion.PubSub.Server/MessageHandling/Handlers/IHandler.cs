using Javithalion.PubSub.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Javithalion.PubSub.Server.MessageHandling.Handlers
{
    public interface IHandler
    {
        Task HandleAsync(Command command);
    }
}
