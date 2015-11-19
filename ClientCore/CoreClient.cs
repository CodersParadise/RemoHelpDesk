namespace ClientCore
{
    using Arrowgene.Services.Common;
    using Arrowgene.Services.Network;
    using Arrowgene.Services.Network.ManagedConnection.Client;
    using Arrowgene.Services.Network.UDP;
    using ClientCore.Handle;
    using ClientCore.Packets;
    using NetworkObjects;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading;

    public class CoreClient
    {
        private const int BC_PORT = 7330;

        private const string assemblyMarrySocket = "Arrowgene.Services";
        private const string assemblyNetworkObjects = "NetworkObjects";

        private HandlePacket handlePacket;
        private ManagedClient managedClient;
        private int port;
        private IPAddress ipAddress;
        private UDPSocket broadcast;

        public delegate void ReceivedChatEventHandler(string foo);
        public event ReceivedChatEventHandler ReceivedChat;

        public delegate void DiscoveredServerEventHandler(IPAddress ipAddress, int port);
        public delegate void ReceivedVoiceEventHandler(byte[] buffer, int length);
        public event DiscoveredServerEventHandler DiscoveredServer;
        public event ReceivedVoiceEventHandler ReceivedVoice;

        public CoreClient()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public bool IsConnected
        {
            get { return this.managedClient.IsConnected; }
        }

        public void Discover()
        {
            this.broadcast.StartReceive();
            this.broadcast.SendBroadcast(System.Text.Encoding.UTF8.GetBytes("HELLO?"), BC_PORT);
        }

        private void Broadcast_ReceivedPacket(object sender, ReceivedUDPPacketEventArgs e)
        {
            string msg = System.Text.Encoding.UTF8.GetString(e.ReadableBuffer.GetBytes());

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
            this.broadcast = new UDPSocket();
            this.broadcast.ReceivedPacket += Broadcast_ReceivedPacket;


            this.managedClient = new ManagedClient(IP.AddressLocalhost(System.Net.Sockets.AddressFamily.InterNetworkV6), 2345);
            this.managedClient.BufferSize = 2000;
            this.managedClient.ReceivedPacket += ManagedClient_ReceivedPacket;
            this.managedClient.Connected += ManagedClient_Connected;

            this.handlePacket = new HandlePacket(this.managedClient.Logger);
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

        protected void OnReceivedVoice(byte[] buffer, int length)
        {
            if (this.ReceivedVoice != null)
            {
                this.ReceivedVoice(buffer, length);
            }
        }

        public void SetHost(IPAddress ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public void Connect()
        {
            this.broadcast.StopReceive();
            this.managedClient.Connect();
        }

        public void Disconnect()
        {
            this.broadcast.StopReceive();
            this.managedClient.Disconnect();
        }

        public void SendChat(string message)
        {
            this.managedClient.SendObject(PacketId.CHAT, message);
        }

        public void SendVoice(byte[] buffer)
        {
            this.managedClient.SendObject(PacketId.VOICE, buffer);
        }


        private void ManagedClient_Connected(object sender, Arrowgene.Services.Network.ManagedConnection.Event.ConnectedEventArgs e)
        {
            this.handlePacket.Send(e.ClientSocket, new SendComputerInfo());
        }

        private void ManagedClient_ReceivedPacket(object sender, Arrowgene.Services.Network.ManagedConnection.Event.ReceivedPacketEventArgs e)
        {
            switch (e.PacketId)
            {
                case PacketId.CHAT:
                    {
                        string message = e.Packet.Object as string;
                        this.OnReceivedChat(message);
                        break;
                    }
                case PacketId.VOICE:
                    {
                        byte[] buffer = e.Packet.Object as byte[];
                        this.OnReceivedVoice(buffer, buffer.Length);
                        break;
                    }
                default:
                    {
                        this.handlePacket.Handle(e.PacketId, e.Packet.Object, e.ClientSocket);
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
