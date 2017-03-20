using Javithalion.PubSub.Client;
using System;

namespace Javithalion.PubSub.ClientUsage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var pubSubClient = new PubSubClient())
            {
                Console.Write("Server's IP: ");
                var ipAddress = Console.ReadLine();

                Console.Write("Server's port: ");
                var port = Convert.ToInt32(Console.ReadLine());

                pubSubClient.Connect(ipAddress, port);

                Console.Write("Topic: ");
                var topic = Console.ReadLine();

                pubSubClient.SubscribeToTopicAsync(topic, (value =>
                {
                    Console.WriteLine();
                    Console.WriteLine($"{DateTime.Now.ToString()} - Message recieved:");
                    Console.WriteLine(value);
                    Console.WriteLine();
                })).ConfigureAwait(false);

                Console.Write("Write 'Exit' to close this client");
                Console.WriteLine();

                string message = string.Empty;
                do
                {
                    Console.Write("Message: ");
                    message = Console.ReadLine();

                    pubSubClient.SendMessageAboutTopicAsync(topic, message).ConfigureAwait(false);

                    System.Threading.Thread.Sleep(50);

                } while (!message.Equals("Exit", StringComparison.CurrentCultureIgnoreCase));
            }
        }
    }
}
