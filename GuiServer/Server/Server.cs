namespace GuiServer.Server
{
    using Arrowgene.Services.Common;
    using Arrowgene.Services.Logging;
    using Arrowgene.Services.Network;
    using Arrowgene.Services.Network.ManagedConnection.Server;
    using Arrowgene.Services.Network.UDP;
    using Database;
    using GuiServer.Server.Events;
    using Http;
    using GuiServer.View.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Windows.Threading;
    using System.Threading;
    using Database.Tables;
    using NetworkObjects;

    public class Server
    {
        private const int BC_PORT = 7330;

        private LogViewModelContainer logViewModelContainer;
        private HandlePacket handlePacket;
        private ClientViewModelContainer clientViewModelContainer;
        private Dispatcher dispatcher;
        private UDPSocket broadcast;
        private RemoHttpServer remoHttp;

        public EventHandler<DisplayTrayBalloonEventArgs> DisplayTrayBalloon;

        public Server(ClientViewModelContainer clientViewModelContainer, LogViewModelContainer logViewModelContainer, Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;

            DatabaseManager dbManager = DatabaseManager.Instance;
            dbManager.CreatDatabase();
            dbManager.CreatTables();

            this.remoHttp = new RemoHttpServer();
            this.remoHttp.Start();

            this.ManagedServer = new ManagedServer(IPAddress.IPv6Any, 2345);
            this.ManagedServer.BufferSize = 2 * 1024 * 1024;

            this.ManagedServer.ReceivedPacket += ManagedServer_ReceivedPacket;
            this.ManagedServer.ClientConnected += ManagedServer_ClientConnected;
            this.ManagedServer.ClientDisconnected += ManagedServer_ClientDisconnected;
            this.ManagedServer.Logger.LogWrite += Logger_LogWrite;
            this.clientViewModelContainer = clientViewModelContainer;
            this.logViewModelContainer = logViewModelContainer;
            this.handlePacket = new HandlePacket(this.clientViewModelContainer, this.dispatcher, this.ManagedServer.Logger, this);

            this.broadcast = new UDPSocket();
            this.broadcast.ReceivedPacket += Broadcast_ReceivedPacket;
        }


        private void Broadcast_ReceivedPacket(object sender, ReceivedUDPPacketEventArgs e)
        {
            string msg = string.Empty;
            IPEndPoint ep = IP.QueryRoutingInterface(IPAddress.Broadcast);

            if (ep.Address != null)
            {
                msg = ep.Address.ToString() + "|" + this.ManagedServer.Port.ToString();
            }
            else
            {
                msg = "CAN_NOT";
            }

            this.broadcast.Send(System.Text.Encoding.UTF8.GetBytes(msg), e.RemoteIPEndPoint);
        }

        public ManagedServer ManagedServer { get; private set; }

        public void Start()
        {
            this.broadcast.StartListen(new IPEndPoint(IPAddress.Any, BC_PORT));
            this.ManagedServer.Start();
        }

        public void Stop()
        {
            this.broadcast.StopReceive();
            this.ManagedServer.Stop();
        }

        private void Logger_LogWrite(object sender, LogWriteEventArgs e)
        {
            this.dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.addLog(new LogViewModel(e.Log));
            }));
        }

        private void ManagedServer_ClientDisconnected(object sender, Arrowgene.Services.Network.ManagedConnection.Event.DisconnectedEventArgs e)
        {
            this.dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                ClientViewModel clientViewModel = this.clientViewModelContainer.GetClientViewModel(e.ClientSocket);
                if (clientViewModel != null)
                {
                    DatabaseManager.Instance.InsertClient(clientViewModel);
                    clientViewModel.Dispose();
                    this.clientViewModelContainer.Remove(clientViewModel);
                }

            }));
        }

        private void ManagedServer_ClientConnected(object sender, Arrowgene.Services.Network.ManagedConnection.Event.ConnectedEventArgs e)
        {

        }

        private void ManagedServer_ReceivedPacket(object sender, Arrowgene.Services.Network.ManagedConnection.Event.ReceivedPacketEventArgs e)
        {
            if (e.PacketId == PacketId.COMPUTER_INFO)
            {
                // special case, we only allow clients with valid computer info
                ComputerInfo computerInfo = e.Packet.GetObject<ComputerInfo>();

                if (computerInfo != null)
                {
                    this.dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        ClientViewModel clientViewModel = new ClientViewModel(e.ClientSocket, computerInfo);
                        this.clientViewModelContainer.Add(clientViewModel);
                    }));
                }
                else
                {
                    e.ClientSocket.Close();
                }
            }
            else
            {
                ClientViewModel clientViewModel = this.clientViewModelContainer.GetClientViewModel(e.ClientSocket);
                this.handlePacket.Handle(e.PacketId, e.Packet.Object, clientViewModel);
            }
        }


        private void addLog(LogViewModel logViewModel)
        {
            logViewModel.CmdClearLog = new CommandHandler(() => this.ClearLog(logViewModel), this.CanClearLog());
            logViewModel.CmdClearAllLog = new CommandHandler(() => this.ClearAllLog(), this.CanClearAllLog());
            this.logViewModelContainer.Add(logViewModel);
        }

        private bool CanClearAllLog()
        {
            if (this.ManagedServer.Logger.Count > 0)
                return true;
            else
                return false;
        }

        private void ClearAllLog()
        {
            this.ManagedServer.Logger.Clear();
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
                this.ManagedServer.Logger.Remove(logViewModel.Id);
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
