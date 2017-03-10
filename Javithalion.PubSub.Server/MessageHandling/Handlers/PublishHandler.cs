using Javithalion.PubSub.Commands;
using Javithalion.PubSub.Server.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Javithalion.PubSub.Server.MessageHandling.Handlers
{
    public class PublishHandler : IHandler
    {
        private readonly Dispatcher _dispatcher;

        public PublishHandler()
        {
            _dispatcher = new Dispatcher();
        }

        public async Task HandleAsync(Command rawCommand)
        {
            var command = (PublishCommand)rawCommand;
            var bodyToSend = Encoding.UTF8.GetBytes(command.RawCommand);

            var subscribers = _dispatcher.GetSubscribers(command.Topic);
            foreach (var subscriber in subscribers)
                try
                {
                    var bytesSent = await Task.Run(() => Listener.Server.SendTo(bodyToSend, SocketFlags.None, subscriber)).ConfigureAwait(false);
                    if (bytesSent <= 0)
                        _dispatcher.RemoveSubscriber(command.Topic, subscriber);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"SendData Error: { ex.Message} UDP Server");
                    _dispatcher.RemoveSubscriber(command.Topic, subscriber);
                }
        }       
    }
}
