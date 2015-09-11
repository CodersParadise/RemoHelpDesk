namespace GuiClient.View.Presenter
{
    using GuiClient.View.Windows;
    using GuiClient.ViewModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;

    public class MainPresenter
    {
        private MainWindow mainWindow;
        private ClientViewModel clientViewModel;
        private ChatPresenter chatPresenter;
        private Button buttonConnect;
        private Button buttonChat;
        private TextBox textBoxIp;
        private Button buttonDiscover;
        private bool buttonConnectIsConnected;
        private int port = 2345;

        public MainPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.AssignControls();
            this.AssignEvents();
            this.PrepareContent();
        }

        private void AssignControls()
        {
            this.buttonConnect = this.mainWindow.btnConnect;
            this.textBoxIp = this.mainWindow.txtIp;
            this.buttonChat = this.mainWindow.btnChat;
            this.buttonDiscover = this.mainWindow.btnDiscover;
        }

        private void AssignEvents()
        {
            this.mainWindow.Closed += mainWindow_Closed;
            this.buttonConnect.Click += btnConnect_Click;
            this.buttonChat.Click += btnChat_Click;
            this.buttonDiscover.Click += buttonDiscover_Click;
        }

        private void buttonDiscover_Click(object sender, RoutedEventArgs e)
        {
            this.StartDiscover();
        }

        private void PrepareContent()
        {
            this.chatPresenter = new ChatPresenter();
            this.clientViewModel = new ClientViewModel();
            this.clientViewModel.DiscoveredServer += clientViewModel_DiscoveredServer;
            this.clientViewModel.SetChatPresenter(this.chatPresenter);
            this.mainWindow.DataContext = this;
            this.buttonConnectIsConnected = false;
            this.buttonConnect.Content = "Connect";
        }

        private void clientViewModel_DiscoveredServer(IPAddress ipAddress, int port)
        {
            Program.DispatchIfNecessary(() =>
            {
                this.textBoxIp.IsEnabled = false;
                this.textBoxIp.Text = ipAddress.ToString();
            });
            this.port = port;

            this.StartClient();
        }


        private void btnChat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.chatPresenter.Show(this.clientViewModel);
        }

        void mainWindow_Closed(object sender, System.EventArgs e)
        {
            this.StopClient();
        }

        void btnConnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.buttonConnectIsConnected)
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
            string ip = null;
            Program.DispatchIfNecessary(() =>
            {
                this.buttonConnect.Content = "Disconnect";
                ip = this.textBoxIp.Text;
            });

            this.buttonConnectIsConnected = true;

            if (!this.clientViewModel.IsConnected && ip != null)
            {
                this.clientViewModel.SetHost(ip, this.port);
                this.clientViewModel.Connect();
            }
        }

        private void StopClient()
        {
            this.buttonConnect.Content = "Connect";
            this.buttonConnectIsConnected = false;

            //     if (this.clientViewModel.IsConnected)
            //    {
            this.clientViewModel.Disconnect();
            //   }
        }

        private void StartDiscover()
        {
            this.clientViewModel.StartDiscover();
        }

        private void StopDiscovery()
        {

        }


    }
}