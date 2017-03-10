using System;
using System.Net;

namespace Javithalion.PubSub.Commands
{
    public abstract class Command
    {
        public Guid Id { get; private set; }

        public DateTime CreationDate { get; private set; }

        public CommandType Type { get; protected set; }

        public EndPoint Endpoint { get; private set; }

        public string Topic { get; protected set; }

        public string RawCommand
        {
            get
            {
                return GetRawCommand();
            }
        }

        public Command(string topic)
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            Topic = topic;
        }

        public Command SetEndpoint(EndPoint endpoint)
        {
            Endpoint = endpoint;
            return this;
        }

        protected abstract string GetRawCommand();
    }

    public enum CommandType
    {
        Publish,
        Subscribe
    }
}
