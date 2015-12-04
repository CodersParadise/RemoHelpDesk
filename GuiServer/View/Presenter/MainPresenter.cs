namespace GuiServer.View
{
    using GuiServer.Server;
    using Server.Database;
    using GuiServer.Server.Events;
    using GuiServer.View.ViewModel;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using System.Net;
    using Arrowgene.Services.Common;

    public class MainPresenter
    {
        private bool isListening;
        private MainWindow mainWindow;
        private Server server;
        private ClientViewModelContainer clientViewModelContainer;
        private LogViewModelContainer logViewModelContainer;
        private ChatViewModelContainer chatViewModelContainer;
        private Dispatcher dispatcher;
        private Button btnListen;
        private ListView lvClients;
        private ListView lvLogs;

        public MainPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            this.SetTitle();

            this.AssignFields();
            this.InitializeViewEvents();
            this.InitializeFields();
            this.AssignView();
            this.InitTrayIcon();
            this.LoadClientsFromDatabase();
        }

        private void SetTitle()
        {
            string title = Program.PROGRAMM_NAME;

            IPEndPoint ep = IP.QueryRoutingInterface(IPAddress.Broadcast);
            if (ep.Address != null)
            {
                title += " [" + ep.Address.ToString() + "]";
            }

            this.mainWindow.Title = title;
        }

        private void LoadClientsFromDatabase()
        {
            DatabaseManager dbManager = DatabaseManager.Instance;

            foreach (ClientViewModel clientViewModel in dbManager.SelectAllClients())
            {
                this.clientViewModelContainer.Add(clientViewModel);
            }
        }

        private void AssignFields()
        {
            this.dispatcher = this.mainWindow.Dispatcher;
            this.btnListen = this.mainWindow.btnListen;
            this.lvClients = this.mainWindow.lvClients;
            this.lvLogs = this.mainWindow.lvLogs;
        }

        private void InitializeViewEvents()
        {
            this.mainWindow.Closed += mainWindow_Closed;
            this.btnListen.Click += btnListen_Click;
            this.mainWindow.StateChanged += mainWindow_StateChanged;
        }

        private void InitializeFields()
        {
            this.logViewModelContainer = new LogViewModelContainer(this.lvLogs);
            this.clientViewModelContainer = new ClientViewModelContainer(this.lvClients);
            this.chatViewModelContainer = ChatViewModelContainer.Instance;
            this.server = new Server(this.chatViewModelContainer, this.clientViewModelContainer, this.logViewModelContainer, this.dispatcher);
            this.server.DisplayTrayBalloon += server_displayTrayBalloon;
            this.isListening = false;
        }

        private void server_displayTrayBalloon(object sender, DisplayTrayBalloonEventArgs e)
        {
            this.SetTrayIcon(e.Title, e.Text);
        }

        private void AssignView()
        {
            this.mainWindow.DataContext = this;
            this.btnListen.Content = "Start Listening";
            this.lvClients.ItemsSource = this.clientViewModelContainer.ClientViewModels;
            this.lvLogs.ItemsSource = this.logViewModelContainer.LogViewModels;
        }

        void btnListen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (isListening)
            {
                this.StopServer();
            }
            else
            {
                this.StartServer();
            }
        }

        public void ShowWindow()
        {
            this.mainWindow.ShowDialog();
        }

        private void ToggleTray(bool showTray)
        {
            if (showTray)
            {
                this.trayIcon.Visible = true;
                this.mainWindow.ShowInTaskbar = false;
            }
            else
            {
                this.trayIcon.Visible = false;
                this.mainWindow.ShowInTaskbar = true;
                this.mainWindow.WindowState = WindowState.Normal;
            }
        }

        private void trayIcon_Click(object sender, EventArgs e)
        {
            this.ToggleTray(false);
        }

        private void mainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.mainWindow.WindowState == WindowState.Minimized)
            {
                this.ToggleTray(true);
            }
            else if (this.mainWindow.WindowState == WindowState.Normal)
            {
                this.ToggleTray(false);
            }
        }

        private void mainWindow_Closed(object sender, System.EventArgs e)
        {
            foreach (ClientViewModel clientViewModel in this.clientViewModelContainer.ClientViewModels)
            {
                clientViewModel.Dispose();
            }

            this.StopServer();
            this.DisposeTrayIcon();
        }

        private void StartServer()
        {
            this.server.Start();
            this.btnListen.Content = "Stop Listening";
            this.isListening = true;
        }

        private void StopServer()
        {
            this.server.Stop();
            this.btnListen.Content = "Start Listening";
            this.isListening = false;
           
        }

        #region tray

        private System.Windows.Forms.NotifyIcon trayIcon;

        private void InitTrayIcon()
        {
            this.trayIcon = new System.Windows.Forms.NotifyIcon();
            this.trayIcon.Text = Program.PROGRAMM_NAME;
            this.trayIcon.Icon = GuiServer.Properties.Resources.icon;
            this.trayIcon.Click += new EventHandler(trayIcon_Click);
        }

        private void SetTrayIcon(string balloonTitle, string balloonText)
        {
            this.trayIcon.BalloonTipTitle = balloonTitle;
            this.trayIcon.BalloonTipText = balloonText;
            this.trayIcon.ShowBalloonTip(500);
        }

        private void DisposeTrayIcon()
        {
            trayIcon.Dispose();
            trayIcon = null;
        }
        #endregion tray

    }
}
