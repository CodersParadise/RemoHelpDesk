namespace ClientCore
{
    using ClientCore.Handle;
    using ClientCore.Packets;
    using MarrySocket.MClient;
    using MarrySocket.MExtra;
    using NetworkObjects;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;

    public class CoreClient
    {
        private const string assemblyMarrySocket = "MarrySocket";
        private const string assemblyNetworkObjects = "NetworkObjects";

        private HandlePacket handlePacket;
        private MarryClient marryClient;
        private ClientConfig clientConfig;

        public delegate void ReceivedChatEventHandler(string foo);
        public event ReceivedChatEventHandler ReceivedChat;


        public CoreClient()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public bool IsConnected
        {
            get { return this.marryClient.IsConnected; }
        }

        public void Init()
        {
            this.clientConfig = new ClientConfig();

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

        public void SetHost(IPAddress ipAddress, int port)
        {
            this.clientConfig.ServerIP = ipAddress;
            this.clientConfig.ServerPort = port;
        }

        public void SetHost(string ipAddress, int port)
        {
            this.SetHost(Maid.IPAddressLookup(ipAddress), port);
        }

        public void Connect()
        {
            this.marryClient.Connect();
        }

        public void Disconnect()
        {
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
