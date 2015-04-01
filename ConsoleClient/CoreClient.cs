namespace ClientCore
{
    using ClientCore.Handle;
    using ClientCore.Packets;
    using MarrySocket.MClient;
    using MarrySocket.MExtra;
    using System;
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

        private void marryClient_Connected(object sender, ConnectedEventArgs e)
        {
            this.handlePacket.Send(this.marryClient.ServerSocket, new SendComputerInfo());
        }

        private void client_ReceivedPacket(object sender, ReceivedPacketEventArgs e)
        {
            this.handlePacket.Handle(e.PacketId, e.MyObject, e.ServerSocket);
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
                Console.WriteLine("Missing Assembly:" + args.Name);
                Console.ReadKey();
                Environment.Exit(0);
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
