using Javithalion.PubSub.Commands;
using Javithalion.PubSub.Commands.Factories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Javithalion.PubSub.Client
{
    public class PubSubClient : IDisposable
    {
        private static bool _continueListening;

        private UdpClient _udpClient;
        private IPEndPoint _remoteEndpoint;
        private IDictionary<string, Action<string>> _subcriptionActions;

        public PubSubClient()
        {
            _continueListening = true;
            _subcriptionActions = new Dictionary<string, Action<string>>();
        }

        public void Connect(string ipAddress, int port)
        {
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));            

            _udpClient.Client.SendBufferSize = 10 * 1024;
            _udpClient.Client.ReceiveBufferSize = 10 * 1024;

            _remoteEndpoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            _continueListening = true;
            Task.Run(async () =>
            {
                while (_continueListening)
                {
                    try
                    {
                        var recievedBytes = await _udpClient.ReceiveAsync();

                        var textMessage = Encoding.UTF8.GetString(recievedBytes.Buffer);
                        var command = CommandFactory.BuildCommand(textMessage);

                        if (_subcriptionActions.ContainsKey(command.Topic))
                            _subcriptionActions[command.Topic](textMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            });
        }

        public void Disconnect()
        {
            _continueListening = false;
        }

        public async Task SubscribeToTopicAsync(string topic, Action<string> callback)
        {
            try
            {
                var command = new SubscribeCommand(topic);

                var bodyToSend = Encoding.UTF8.GetBytes(command.RawCommand);
                var header = BitConverter.GetBytes(bodyToSend.Length + 4);
                var bytesToSend = header.Concat(bodyToSend).ToArray();

                if (_subcriptionActions.ContainsKey(topic))
                    _subcriptionActions[topic] = callback;
                else
                    _subcriptionActions.Add(topic, callback);

                await _udpClient.SendAsync(bytesToSend, bytesToSend.Length, _remoteEndpoint);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Problem subscribing to {0} reason was {1}", topic, exc.ToString());
            }
        }

        public async Task SendMessageAboutTopicAsync(string topic, string message)
        {
            try
            {
                var command = new PublishCommand(topic, message);

                var bodyToSend = Encoding.UTF8.GetBytes(command.RawCommand);
                var header = BitConverter.GetBytes(bodyToSend.Length + 4);
                var bytesToSend = header.Concat(bodyToSend).ToArray();

                await _udpClient.SendAsync(bytesToSend, bytesToSend.Length, _remoteEndpoint);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Problem publishing about {0} reason was {1}", topic, exc.ToString());
            }
        }

        #region IDisposable
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect();
                    _remoteEndpoint = null;

                    _udpClient.Dispose();
                    _udpClient = null;

                    _subcriptionActions.Clear();
                    _subcriptionActions = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
