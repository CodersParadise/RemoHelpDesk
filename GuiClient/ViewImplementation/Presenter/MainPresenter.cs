using GuiClient.ClientImplementation;
using GuiClient.ClientImplementation.ViewModel;
using GuiClient.ViewImplementation.Windows;
using MarrySocket.MClient;
using MarrySocket.MExtra;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GuiClient.ViewImplementation.Presenter
{
    public class MainPresenter
    {
        private MainWindow mainWindow;
        private ChatPresenter chatPresenter;
        private Client client;
        private ClientConfig clientConfig;
        private Thread clientThread;
        private Button buttonConnect;
        private Button buttonChat;
        private TextBox textBoxIp;
      

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
            this.buttonConnect = this.mainWindow.btnConnect;
            this.textBoxIp = this.mainWindow.txtIp;
            this.buttonChat = this.mainWindow.btnChat;
        }

        private void InitializeViewEvents()
        {
            this.mainWindow.Closed += mainWindow_Closed;
            this.buttonConnect.Click += btnConnect_Click;
            this.buttonChat.Click += btnChat_Click;
        }

        private void InitializeFields()
        {
            this.clientConfig = new ClientConfig();
            this.client = new Client(this.clientConfig);
        }

        private void btnChat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChatWindow chatWindow = new ChatWindow();
            chatPresenter = new ChatPresenter(chatWindow, this.client);
            chatPresenter.Show();
        }

        private void AssignView()
        {
            this.mainWindow.DataContext = this;
        }

        void mainWindow_Closed(object sender, System.EventArgs e)
        {
            this.StopClient();
        }

        void btnConnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            if(this.client)
            this.StartClient();
        }

        public void ShowWindow()
        {
            this.mainWindow.ShowDialog();
        }

        private void StartClient()
        {
            this.clientThread = new Thread(this.client.Run);


            this.clientConfig.ServerIP = Maid.IPAddressLookup(this.textBoxIp.Text);

            if (this.clientConfig.ServerPort != null)
            {
                this.buttonConnect.Content = "Disconnect";



                this.clientConfig.ServerPort = 2345;
                this.clientThread.Start();
            }
            else
            {
                MessageBox.Show("Ip adresse ist nicht gültig.");
            }
        }

        private void StopClient()
        {
            //Muss aufjedenfall disconnected werden
            try
            {
                this.clientThread.Join();
                this.client.Disconnect("Client Stopped");
            }
            catch
            {
                this.clientThread.Abort();
            }

            this.buttonConnect.Content = "Connect";
        }

    }
}