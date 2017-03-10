namespace Javithalion.PubSub.Commands
{
    public class SubscribeCommand : Command
    {
        private const string CommandPattern = "Subscribe|{0}|";

        public SubscribeCommand(string topic)
            : base(topic)
        {
            Type = CommandType.Subscribe;
        }

        protected override string GetRawCommand()
        {
            return string.Format(CommandPattern, Topic);
        }
    }
}
