namespace GuiServer.View
{
    using GuiServer.Server;
    using GuiServer.View.ViewModel;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using System.Windows.Threading;

    public class MainPresenter
    {

        private bool isListening;
        private MainWindow mainWindow;
        private Server server;
        private ClientViewModelContainer clientViewModelContainer;
        private LogViewModelContainer logViewModelContainer;
        private Dispatcher dispatcher;
        private Button btnListen;
        private ListView lvClients;
        private ListView lvLogs;

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
            this.btnListen = this.mainWindow.btnListen;
            this.lvClients = this.mainWindow.lvClients;
            this.lvLogs = this.mainWindow.lvLogs;
        }

        private void InitializeViewEvents()
        {
            this.mainWindow.Closed += mainWindow_Closed;
            this.btnListen.Click += btnListen_Click;
        }

        private void InitializeFields()
        {
            this.logViewModelContainer = new LogViewModelContainer(this.lvLogs);
            this.clientViewModelContainer = new ClientViewModelContainer(this.lvClients);
            this.server = new Server(this.clientViewModelContainer, this.logViewModelContainer, this.dispatcher);
            this.isListening = false;
        }

        private void AssignView()
        {
            this.mainWindow.DataContext = this;
            this.btnListen.Content = "Start Listening";
            this.lvClients.ItemsSource = this.clientViewModelContainer.ClientViewModels;
            this.lvLogs.ItemsSource = this.logViewModelContainer.LogViewModels;
        }

        void mainWindow_Closed(object sender, System.EventArgs e)
        {
            this.StopServer();
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
            this.clientViewModelContainer.Clear();
        }

    }
}
