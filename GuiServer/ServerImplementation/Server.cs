namespace GuiServer.ServerImplementation
{
    using GuiServer.ServerImplementation.ViewModel;
    using MarrySocket.MExtra.Logging;
    using MarrySocket.MServer;
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Threading;

    public class Server
    {
        private ObservableCollection<LogViewModel> logViewModels;
        private HandlePacket handlePacket;
        private ClientViewModelContainer clientViewModelContainer;
        private Dispatcher dispatcher;
        private MarryServer marryServer;

        public Server(ClientViewModelContainer clientViewModelContainer, ObservableCollection<LogViewModel> logViewModels, Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            ServerConfig config = new ServerConfig();
            config.BufferSize = 700000;
            this.marryServer = new MarryServer(config);
            this.marryServer.ReceivedPacket += marryServer_ReceivedPacket;
            this.marryServer.ClientConnected += marryServer_ClientConnected;
            this.marryServer.ClientDisconnected += marryServer_ClientDisconnected;
            this.marryServer.Logger.LogWrite += Logger_LogWrite;
            this.clientViewModelContainer = clientViewModelContainer;
            this.logViewModels = logViewModels;
            this.handlePacket = new HandlePacket(this.clientViewModelContainer, this.dispatcher, this.marryServer.Logger);
        }

        public void Start()
        {
            this.marryServer.Start();
        }

        public void Stop()
        {
            this.marryServer.Stop();
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

            this.dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.addLog(new LogViewModel(new Log("Packet Arrived!")));
            }));

            ClientViewModel clientViewModel = this.clientViewModelContainer.GetClientViewModel(e.ServerSocket);
            this.handlePacket.Handle(e.PacketId, e.MyObject, clientViewModel);
        }

        private void addLog(LogViewModel logViewModel)
        {
            logViewModel.CmdClearLog = new CommandHandler(() => this.ClearLog(logViewModel), this.CanClearLog());
            logViewModel.CmdClearAllLog = new CommandHandler(() => this.ClearAllLog(), this.CanClearAllLog());
            this.logViewModels.Add(logViewModel);
        }

        private bool CanClearAllLog()
        {
            if (this.marryServer.Logger.Count > 0)
                return true;
            else
                return false;
        }

        private void ClearAllLog()
        {
            this.marryServer.Logger.Clear();
            this.logViewModels.Clear();
        }

        private bool CanClearLog()
        {
            return true;
        }

        private void ClearLog(LogViewModel logViewModel)
        {
            if (logViewModel != null)
            {
                this.marryServer.Logger.Remove(logViewModel.Id);
                this.logViewModels.Remove(logViewModel);
            }
        }

    }
}
