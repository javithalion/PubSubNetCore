using Javithalion.PubSub.Commands.Factories;
using Javithalion.PubSub.Server.MessageHandling.Factories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Javithalion.PubSub.Server.Server
{
    public class Listener : IDisposable
    {
        private bool _continueListening;

        public static Socket Server;

        public void StartListening(int port)
        {
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ConfigureSocketLowLevelOptions();

            Server.Bind(new IPEndPoint(IPAddress.Any, port));
            var commandState = new CommandState();

            //Start listening for a new message.     
            _continueListening = true;
            Task.Run(() =>
            {
                while (_continueListening)
                {
                    try
                    {
                        EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                        var recievedBytes = Server.ReceiveFrom(commandState.Buffer, SocketFlags.Peek, ref newClientEP);

                        var lenght = BitConverter.ToInt32(commandState.Buffer, 0);
                        commandState.SetLength(lenght);
                        recievedBytes = Server.ReceiveFrom(commandState.Buffer, SocketFlags.None, ref newClientEP);

                        var message = Encoding.UTF8.GetString(commandState.Buffer.Skip(4).ToArray());
                        var command = CommandFactory.BuildCommand(message);
                        command.SetEndpoint(newClientEP);

                        var handler = HandlerFactory.GetHandlerForCommand(command);
                        handler.Handle(command);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine($"Linstener error: {exc.Message} UDP Server. {exc.ToString()}");
                    }
                    finally
                    {
                        commandState.ResetCommand();
                    }
                }
            });
        }

        private static void ConfigureSocketLowLevelOptions()
        {
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

            Server.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
        }

        private void Disconnect()
        {
            _continueListening = false;
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

                    Server.Dispose();
                    Server = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        private class CommandState
        {
            public byte[] Buffer { get; set; }
            public int Length { get; private set; }
            public bool ReadHeaderPending { get; private set; }
            public int ReadedBytes { get; private set; }

            public CommandState()
            {
                ResetCommand();
            }

            public CommandState ResetCommand()
            {
                Length = 1024 * 3;
                Buffer = new byte[Length];
                ReadHeaderPending = true;
                ReadedBytes = 0;

                return this;
            }

            public CommandState SetLength(int length)
            {
                Length = length;
                Buffer = new byte[Length];

                return this;
            }

            public CommandState HeaderWasRead()
            {
                ReadHeaderPending = false;

                return this;
            }

            public CommandState NewAmountOfBytesRead(int amountOfBytes)
            {
                ReadedBytes += amountOfBytes;

                return this;
            }
        }
    }
}
