using Javithalion.PubSub.Commands;
using Javithalion.PubSub.Server.MessageHandling.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Javithalion.PubSub.Server.MessageHandling.Factories
{
    public class HandlerFactory
    {
        public static IHandler GetHandlerForCommand(Command command)
        {
            switch(command.Type)
            {
                case CommandType.Publish:
                    return new SubscribeHandler();
                case CommandType.Subscribe:
                    return new PublishHandler();
                default:
                    throw new InvalidOperationException("Not suported command");

            }
        }
    }
}
