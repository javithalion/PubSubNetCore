namespace Javithalion.PubSub.Commands
{
    public class PublishCommand : Command
    {
        private const string CommandPattern = "Publish|{0}|{1}";

        public string Message { get; private set; }

        public PublishCommand(string topic, string message)
            : base(topic)
        {
            Type = CommandType.Publish;
            Message = message;
        }

        protected override string GetRawCommand()
        {
            return string.Format(CommandPattern, Topic, Message);
        }
    }
}
