using System;

namespace Javithalion.PubSub.Commands.Factories
{
    public class CommandFactory
    {
        public static Command BuildCommand(string rawCommand)
        {
            var rawCommandSplit = rawCommand.Split('|');

            switch (rawCommandSplit[0].ToLower())
            {
                case "publish":
                    string publishTopic = rawCommandSplit[1];
                    string publishMessage = rawCommandSplit[2];
                    return new PublishCommand(publishTopic, publishMessage);

                case "subscribe":
                    string subscribeTopic = rawCommandSplit[1];
                    return new SubscribeCommand(subscribeTopic);

                default:
                    throw new InvalidOperationException($"Unknown command '{rawCommandSplit[0]}'");
            }
        }
    }
}
