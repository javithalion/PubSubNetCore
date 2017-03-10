using Javithalion.PubSub.Server.Server;
using System;

namespace Javithalion.PubSub.ServerUsage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var pubSubServer = new Listener())
            {
                pubSubServer.StartListening(10050);

                Console.Write("Press any key to stop the server...");
                Console.ReadKey();
            }
        }
    }
}
