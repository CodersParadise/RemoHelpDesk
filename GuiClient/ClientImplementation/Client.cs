
using GuiClient.ClientImplementation;


using MarrySocket.MClient;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

using GuiClient.ClientImplementation.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
namespace GuiClient.ClientImplementation
{
    class Client
    {

         private const string SERVER_HOST = "localhost";
        private const int SERVER_PORT = 2345;
        private const int RECONNECT_TIMEOUT_MS = 1000;
        private static HandlePacket handlePacket;
        private static MarryClient client;
        private const string assemblyMarrySocket = "MarrySocket";
        private const string assemblyNetworkObjects = "NetworkObjects";

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Run();
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
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ConsoleClient." + name + ".dll"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        private static void Run()
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
                    IPAddress ipAdress = Maid.IPAddressLookup(SERVER_HOST);

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

        private static void client_Disconnected(object sender, DisconnectedEventArgs e)
        {

        }

        private static void client_Connected(object sender, ConnectedEventArgs e)
        {
            handlePacket.Send(client.ServerSocket, new SendComputerInfo());
        }

        private static void Logger_LogWrite(object sender, MarrySocket.MExtra.Logging.LogWriteEventArgs e)
        {
            Console.WriteLine(e.Log.Text);
        }

        private static void client_ReceivedPacket(object sender, ReceivedPacketEventArgs e)
        {
            handlePacket.Handle(e.PacketId, e.MyObject, e.ServerSocket);
        }
    
    }
}
