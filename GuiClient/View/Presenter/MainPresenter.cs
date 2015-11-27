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
        private int port = 2345;
        private IPAddress ipAddress;

        public MainPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.AssignControls();
            this.PrepareContent();
            this.AssignEvents();
        }

        private void AssignControls()
        {
            this.buttonConnect = this.mainWindow.btnConnect;
            this.textBoxIp = this.mainWindow.txtIp;
            this.buttonChat = this.mainWindow.btnChat;
            this.buttonDiscover = this.mainWindow.btnDiscover;
        }

        private void PrepareContent()
        {
            this.chatPresenter = new ChatPresenter();
            this.clientViewModel = new ClientViewModel();
            this.clientViewModel.DiscoveredServer += clientViewModel_DiscoveredServer;
            this.clientViewModel.DisconnectedServer += ClientViewModel_DisconnectedServer;
            this.clientViewModel.SetChatPresenter(this.chatPresenter);
            this.mainWindow.DataContext = this;
            this.buttonConnect.Content = "Connect";
            this.ipAddress = null;
            this.buttonChat.IsEnabled = false;
        }

        private void ClientViewModel_DisconnectedServer()
        {
            this.StopClient();
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

        private void btnChat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.chatPresenter.Show(this.clientViewModel);
        }

        private void mainWindow_Closed(object sender, System.EventArgs e)
        {
            this.StopClient();
        }

        private void clientViewModel_DiscoveredServer(IPAddress ipAddress, int port)
        {
            this.port = port;
            this.ipAddress = ipAddress;
            this.StartClient();
        }

        private void btnConnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ipAddress = null;
            if (this.clientViewModel.IsConnected)
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
            if (!this.clientViewModel.IsConnected)
            {
                if (this.ipAddress == null)
                {
                    string ip = this.textBoxIp.Text;
                    this.ipAddress = IPAddress.Parse(ip);
                }

                this.clientViewModel.SetHost(this.ipAddress, this.port);
                this.clientViewModel.Connect();
            }

            if (this.clientViewModel.IsConnected)
            {
                Program.DispatchIfNecessary(() =>
                {
                    this.buttonConnect.Content = "Disconnect";
                    this.textBoxIp.IsEnabled = false;
                    this.textBoxIp.Text = this.ipAddress.ToString();
                    this.buttonDiscover.IsEnabled = false;
                    this.buttonChat.IsEnabled = true;
                });
            }
            else
            {
                Program.DispatchIfNecessary(() =>
                {
                    MessageBox.Show("Verbindung fehlgeschlagen");
                });
            }
        }


        private void StopClient()
        {
            this.clientViewModel.Disconnect();
            Program.DispatchIfNecessary(() =>
            {
                this.buttonConnect.Content = "Connect";
                this.textBoxIp.IsEnabled = true;
                this.buttonDiscover.IsEnabled = true;
                this.buttonChat.IsEnabled = false;
            });
        }

        private void StartDiscover()
        {
            this.clientViewModel.StartDiscover();
        }

    }
}