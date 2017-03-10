using Javithalion.PubSub.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Javithalion.PubSub.Server.MessageHandling.Handlers
{
    public class SubscribeHandler : IHandler
    {
        private readonly Dispatcher _dispatcher;

        public SubscribeHandler()
        {
            _dispatcher = new Dispatcher();
        }

        public async Task HandleAsync(Command rawCommand)
        {
            var command = (SubscribeCommand)rawCommand;
            await Task.Run(() => _dispatcher.AddSubscriber(command.Topic, command.Endpoint)).ConfigureAwait(false);
        }
    }
}