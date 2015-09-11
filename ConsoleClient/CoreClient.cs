namespace ClientCore
{

    using Arrowgene.Services.Network;
    using Arrowgene.Services.Network.Broadcast;
    using Arrowgene.Services.Network.MarrySocket.MClient;
    using ClientCore.Handle;
    using ClientCore.Packets;
    using NetworkObjects;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;

    public class CoreClient
    {
        private const int CLIENT_BC_PORT = 7331;
        private const int SERVER_BC_PORT = 7330;

        private const string assemblyMarrySocket = "Arrowgene.Services";
        private const string assemblyNetworkObjects = "NetworkObjects";

        private HandlePacket handlePacket;
        private MarryClient marryClient;
        private ClientConfig clientConfig;
        private UDPBroadcast broadcast;

        public delegate void ReceivedChatEventHandler(string foo);
        public event ReceivedChatEventHandler ReceivedChat;

        public delegate void DiscoveredServerEventHandler(IPAddress ipAddress, int port);
        public event DiscoveredServerEventHandler DiscoveredServer;

        public CoreClient()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public bool IsConnected
        {
            get { return this.marryClient.IsConnected; }
        }

        public void Discover()
        {
            UDPBroadcast broadcast = new UDPBroadcast(CoreClient.SERVER_BC_PORT);
            broadcast.Send(System.Text.Encoding.UTF8.GetBytes("HELLO?"));
        }

        private void broadcast_ReceivedBroadcast(object sender, Arrowgene.Services.Network.Discovery.ReceivedUDPBroadcastPacketEventArgs e)
        {
            string msg = System.Text.Encoding.UTF8.GetString(e.Payload.GetBytes());

            if (msg.Contains("|"))
            {
                string[] epInfo = msg.Split('|');

                IPAddress serverIp = null;
                IPAddress.TryParse(epInfo[0], out serverIp);
                int serverPort = 0;
                int.TryParse(epInfo[1], out serverPort);

                if (serverPort != 0 && serverIp != null)
                {
                    this.OnDiscoveredServer(serverIp, serverPort);
                }
            }
        }

        public void Init()
        {
            this.clientConfig = new ClientConfig(IP.AddressLocalhost(System.Net.Sockets.AddressFamily.InterNetworkV6), 2345);

            this.broadcast = new UDPBroadcast(CoreClient.CLIENT_BC_PORT);
            this.broadcast.ReceivedBroadcast += broadcast_ReceivedBroadcast;
            this.broadcast.Listen();

            this.marryClient = new MarryClient(this.clientConfig);
            this.marryClient.ReceivedPacket += client_ReceivedPacket;
            this.marryClient.Connected += marryClient_Connected;

            this.handlePacket = new HandlePacket(this.marryClient.Logger);
        }

        protected void OnReceivedChat(string message)
        {
            if (this.ReceivedChat != null)
            {
                this.ReceivedChat(message);
            }
        }

        protected void OnDiscoveredServer(IPAddress ipAddress, int port)
        {
            if (this.DiscoveredServer != null)
            {
                this.DiscoveredServer(ipAddress, port);
            }
        }

        public void SetHost(IPAddress ipAddress, int port)
        {
            this.clientConfig.ServerIP = ipAddress;
            this.clientConfig.ServerPort = port;
        }

        public void SetHost(string ipAddress, int port)
        {
            this.SetHost(IP.AddressLookup(ipAddress), port);
        }

        public void Connect()
        {
            this.broadcast.Stop();
            this.marryClient.Connect();
        }

        public void Disconnect()
        {
            this.broadcast.Stop();
            this.marryClient.Disconnect();
        }

        public void SendChat(string message)
        {
            this.marryClient.ServerSocket.SendObject(PacketId.CHAT, message);
        }

        private void marryClient_Connected(object sender, ConnectedEventArgs e)
        {
            this.handlePacket.Send(this.marryClient.ServerSocket, new SendComputerInfo());
        }

        private void client_ReceivedPacket(object sender, ReceivedPacketEventArgs e)
        {
            switch (e.PacketId)
            {
                case PacketId.CHAT:
                    {
                        string message = e.MyObject as string;
                        this.OnReceivedChat(message);
                        break;
                    }
                default:
                    {
                        this.handlePacket.Handle(e.PacketId, e.MyObject, e.ServerSocket);
                        break;
                    }
            }

        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            if (args.Name.Contains(assemblyMarrySocket))
            {
                assembly = LoadAssembly(assemblyMarrySocket);
            }
            else if (args.Name.Contains(assemblyNetworkObjects))
            {
                assembly = LoadAssembly(assemblyNetworkObjects);
            }
            else
            {
                Debug.WriteLine("Missing Assembly:" + args.Name);
            }
            return assembly;
        }

        private static Assembly LoadAssembly(string name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClientCore." + name + ".dll"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}
