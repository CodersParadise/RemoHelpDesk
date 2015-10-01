namespace GuiServer.Server
{
    using Arrowgene.Services.Logging;
    using Arrowgene.Services.Network;
    using Arrowgene.Services.Network.MarrySocket.MServer;
    using Arrowgene.Services.Network.UDP;
    using GuiServer.Server.Events;
    using GuiServer.View.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Windows.Threading;

    public class Server
    {
        private const int SERVER_BC_PORT = 7330;

        private LogViewModelContainer logViewModelContainer;
        private HandlePacket handlePacket;
        private ClientViewModelContainer clientViewModelContainer;
        private Dispatcher dispatcher;
       // private UDPSocket broadcast;
        private ServerConfig serverConfig;
        public EventHandler<DisplayTrayBalloonEventArgs> DisplayTrayBalloon;

        public Server(ClientViewModelContainer clientViewModelContainer, LogViewModelContainer logViewModelContainer, Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            serverConfig = new ServerConfig(IPAddress.IPv6Any, 2345);
            serverConfig.BufferSize = 2 * 1024 * 1024;
            this.MarryServer = new MarryServer(serverConfig);
            this.MarryServer.ReceivedPacket += marryServer_ReceivedPacket;
            this.MarryServer.ClientConnected += marryServer_ClientConnected;
            this.MarryServer.ClientDisconnected += marryServer_ClientDisconnected;
            this.MarryServer.Logger.LogWrite += Logger_LogWrite;
            this.clientViewModelContainer = clientViewModelContainer;
            this.logViewModelContainer = logViewModelContainer;
            this.handlePacket = new HandlePacket(this.clientViewModelContainer, this.dispatcher, this.MarryServer.Logger, this);
          //  this.broadcast = new UDPServer(SERVER_BC_PORT);
         //   this.broadcast.ReceivedPacket += Broadcast_ReceivedPacket;
        }

        private void Broadcast_ReceivedPacket(object sender, ReceivedUDPPacketEventArgs e)
        {
            string msg = string.Empty;
            IPEndPoint ep = IP.QueryRoutingInterface(IPAddress.Broadcast);

            if (ep.Address != null)
            {
                msg = ep.Address.ToString() + "|" + this.serverConfig.ServerPort.ToString();
            }
            else
            {
                msg = "CAN_NOT";
            }

         //   this.broadcast.SendTo(System.Text.Encoding.UTF8.GetBytes(msg), e.RemoteIPEndPoint);
        }

        public MarryServer MarryServer;

        public void Start()
        {
         //   this.broadcast.Listen();
            this.MarryServer.Start();
        }

        public void Stop()
        {
          //  this.broadcast.Stop();
            this.MarryServer.Stop();
        }

        private void Logger_LogWrite(object sender, LogWriteEventArgs e)
        {
            this.dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.addLog(new LogViewModel(e.Log));
            }));
        }

        private void marryServer_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            this.dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                ClientViewModel clientViewModel = this.clientViewModelContainer.GetClientViewModel(e.ClientSocket);
                clientViewModel.Dispose();
                this.clientViewModelContainer.Remove(clientViewModel);
            }));
        }

        private void marryServer_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            this.dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.clientViewModelContainer.Add(new ClientViewModel(e.ClientSocket));
            }));
        }

        private void marryServer_ReceivedPacket(object sender, ReceivedPacketEventArgs e)
        {
            ClientViewModel clientViewModel = this.clientViewModelContainer.GetClientViewModel(e.ClientSocket);
            this.handlePacket.Handle(e.PacketId, e.MyObject, clientViewModel);

        }

        private void addLog(LogViewModel logViewModel)
        {
            logViewModel.CmdClearLog = new CommandHandler(() => this.ClearLog(logViewModel), this.CanClearLog());
            logViewModel.CmdClearAllLog = new CommandHandler(() => this.ClearAllLog(), this.CanClearAllLog());
            this.logViewModelContainer.Add(logViewModel);
        }

        private bool CanClearAllLog()
        {
            if (this.MarryServer.Logger.Count > 0)
                return true;
            else
                return false;
        }

        private void ClearAllLog()
        {
            this.MarryServer.Logger.Clear();
            this.logViewModelContainer.Clear();
        }

        private bool CanClearLog()
        {
            return true;
        }

        private void ClearLog(LogViewModel logViewModel)
        {
            if (logViewModel != null)
            {
                this.MarryServer.Logger.Remove(logViewModel.Id);
                this.logViewModelContainer.Remove(logViewModel);
            }
        }

        public void RaiseDisplayTrayBalloon(string title, string text)
        {
            if (this.DisplayTrayBalloon != null)
            {
                this.DisplayTrayBalloon(this, new DisplayTrayBalloonEventArgs(title, text));
            }
        }

    }
}
