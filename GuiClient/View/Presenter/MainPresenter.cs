namespace GuiClient.View.Presenter
{
    using GuiClient.View.Windows;
    using GuiClient.ViewModel;
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
        private bool buttonConnectIsConnected;

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
        }

        private void AssignEvents()
        {
            this.mainWindow.Closed += mainWindow_Closed;
            this.buttonConnect.Click += btnConnect_Click;
            this.buttonChat.Click += btnChat_Click;
        }

        private void PrepareContent()
        {
            this.clientViewModel = new ClientViewModel();
            this.mainWindow.DataContext = this;
            this.buttonConnectIsConnected = false;
            this.buttonConnect.Content = "Connect";
        }


        private void btnChat_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChatWindow chatWindow = new ChatWindow();
            this.chatPresenter = new ChatPresenter(chatWindow);
            this.chatPresenter.Show();
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
            this.buttonConnect.Content = "Disconnect";
            this.buttonConnectIsConnected = true;

            if (this.clientViewModel.IsConnected)
            {
                MessageBox.Show("Already Connected");
            }
            else
            {
                this.clientViewModel.SetHost(this.textBoxIp.Text, 2345);
                this.clientViewModel.Connect();
            }
        }

        private void StopClient()
        {
            this.buttonConnect.Content = "Connect";
            this.buttonConnectIsConnected = false;

            if (this.clientViewModel.IsConnected)
            {
                this.clientViewModel.Disconnect();
            }
            else
            {
                MessageBox.Show("Already Disconnected");
            }

        }

    }
}