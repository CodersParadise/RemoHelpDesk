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
        private LogViewModelContainer logViewModelContainer;
        private HandlePacket handlePacket;
        private ClientViewModelContainer clientViewModelContainer;
        private Dispatcher dispatcher;

        public Server(ClientViewModelContainer clientViewModelContainer, LogViewModelContainer logViewModelContainer, Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            ServerConfig config = new ServerConfig();
            config.BufferSize = 2 * 1024 * 1024;
            this.MarryServer = new MarryServer(config);
            this.MarryServer.ReceivedPacket += marryServer_ReceivedPacket;
            this.MarryServer.ClientConnected += marryServer_ClientConnected;
            this.MarryServer.ClientDisconnected += marryServer_ClientDisconnected;
            this.MarryServer.Logger.LogWrite += Logger_LogWrite;
            this.clientViewModelContainer = clientViewModelContainer;
            this.logViewModelContainer = logViewModelContainer;
            this.handlePacket = new HandlePacket(this.clientViewModelContainer, this.dispatcher, this.MarryServer.Logger);
        }

        public MarryServer MarryServer;

        public void Start()
        {
            this.MarryServer.Start();
        }

        public void Stop()
        {
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
            ClientViewModel clientViewModel = this.clientViewModelContainer.GetClientViewModel(e.ServerSocket);
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

    }
}
