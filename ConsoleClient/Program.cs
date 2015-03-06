namespace ConsoleClient
{
    using ConsoleClient.Packets;
    using MarrySocket.MClient;
    using MarrySocket.MExtra;
    using MarrySocket.MExtra.Serialization;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public static class Program
    {
        private const string SERVER_HOST = "localhost";
        private const int SERVER_PORT = 2345;
        private const int RECONNECT_TIMEOUT_MS = 1000;
        private static HandlePacket handlePacket;
        private static MarryClient client;

        static void Main(string[] args)
        {
            ClientConfig config = new ClientConfig();
            client = new MarryClient(config);
            handlePacket = new HandlePacket(client.Logger);

            client.Connected += client_Connected;
            client.ReceivedPacket += client_ReceivedPacket;
            client.Disconnected += client_Disconnected;
            client.Logger.LogWrite += Logger_LogWrite;

            while (true)
            {
                if (!client.IsConnected)
                {
                    IPAddress ipAdress = Maid.IPAddressLookup(SERVER_HOST, AddressFamily.InterNetworkV6);

                    if (ipAdress != null)
                    {
                        config.ServerIP = ipAdress;
                        config.ServerPort = SERVER_PORT;
                        client.Connect();
                    }

                    Thread.Sleep(RECONNECT_TIMEOUT_MS);
                }
            }
        }

        static void client_Disconnected(object sender, DisconnectedEventArgs e)
        {

        }

        static void client_Connected(object sender, ConnectedEventArgs e)
        {
            handlePacket.Send(client.ServerSocket, new SendComputerInfo());
        }

        static void Logger_LogWrite(object sender, MarrySocket.MExtra.Logging.LogWriteEventArgs e)
        {
            Console.WriteLine(e.Log.Text);
        }

        static void client_ReceivedPacket(object sender, ReceivedPacketEventArgs e)
        {
            handlePacket.Handle(e.PacketId, e.MyObject, e.ServerSocket);
        }
    }
}
