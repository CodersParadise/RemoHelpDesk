﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiClient.ViewImplementation.Presenter
{
    using GuiClient.ClientImplementation;
    using GuiClient.ClientImplementation.ViewModel;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using System.Windows.Threading;

    public class MainPresenter
    {

        private MainWindow mainWindow;
        private Client client;

        private Dispatcher dispatcher;
        private Button btnConnect;
        private TextBox txtIp;
        private bool isConnected;


        public MainPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            this.AssignFields();
            this.InitializeViewEvents();
            this.InitializeFields();
            this.AssignView();
        }

        private void AssignFields()
        {
            this.dispatcher = this.mainWindow.Dispatcher;
            this.btnConnect = this.mainWindow.btnConnect;

            this.txtIp = this.mainWindow.txtIp;

        }

        private void InitializeViewEvents()
        {
            this.mainWindow.Closed += mainWindow_Closed;
            this.btnConnect.Click += btnConnect_Click;
        }

        private void InitializeFields()
        {

            //  this.server = new Server(this.clientViewModelContainer, this.logViewModelContainer, this.dispatcher);

        }

        private void AssignView()
        {
            this.mainWindow.DataContext = this;
            this.isConnected = false;

            //    this.lvClients.ItemsSource = this.clientViewModelContainer.ClientViewModels;
            //     this.lvLogs.ItemsSource = this.logViewModelContainer.LogViewModels;
        }

        void mainWindow_Closed(object sender, System.EventArgs e)
        {
            this.StopClient();
            this.isConnected = false;

        }

        void btnConnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (isConnected)
            {
                 this.StopClient();
            }
            else
            {
                this.StartClient();
            }
        }

        public void ShowWindow()
        {
            this.mainWindow.ShowDialog();
        }

        private void StartClient()
        {
            this.isConnected = true;
        }

        private void StopClient()
        {
            this.isConnected = false;

        }
    }
}