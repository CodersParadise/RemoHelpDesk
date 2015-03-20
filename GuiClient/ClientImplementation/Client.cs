namespace GuiClient.ClientImplementation
{
    using MarrySocket.MClient;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Threading;
    using GuiClient.ClientImplementation.ViewModel;
    using MarrySocket.MExtra;
    using ConsoleClient.Handle;
    using System;
    using ConsoleClient.Packets;

    public class Client
    {
        private HandlePacket handlePacket;
        private ClientConfig clientConfig;

        public Client(ClientConfig clientConfig)
        {
            this.clientConfig = clientConfig;
            this.MarryClient = new MarryClient(this.clientConfig);
            this.handlePacket = new HandlePacket(this.MarryClient.Logger);
            this.MarryClient.Connected += client_Connected;
            this.MarryClient.ReceivedPacket += client_ReceivedPacket;
            this.MarryClient.Disconnected += client_Disconnected;
            this.MarryClient.Logger.LogWrite += Logger_LogWrite;
        }

        public ServerSocketViewModel ServerSocketViewModel { get; private set; }
        public MarryClient MarryClient { get; private set; }

        public void Run()
        {
            if (!this.MarryClient.IsConnected)
            {
                this.MarryClient.Connect();
            }
        }

        public void Disconnect(String pReason)
        {
            this.MarryClient.Disconnect(pReason);

        }

        private void client_Disconnected(object sender, DisconnectedEventArgs e)
        {
      
        }

        private void client_Connected(object sender, ConnectedEventArgs e)
        {
            this.ServerSocketViewModel = new ServerSocketViewModel(this.MarryClient.ServerSocket);
            this.handlePacket.Send(this.MarryClient.ServerSocket, new SendComputerInfo());
        }

        private void Logger_LogWrite(object sender, MarrySocket.MExtra.Logging.LogWriteEventArgs e)
        {
            Console.WriteLine(e.Log.Text);
        }

        private void client_ReceivedPacket(object sender, ReceivedPacketEventArgs e)
        {
            this.handlePacket.Handle(e.PacketId, e.MyObject, this.ServerSocketViewModel);
        }

    }
}
