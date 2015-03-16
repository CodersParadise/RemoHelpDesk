using GuiClient.ClientImplementation;
using GuiClient.ClientImplementation.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace GuiClient.ViewImplementation.Presenter
{


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
            this.client = new Client();
          

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
       
                Thread thread1 = new Thread(new ParameterizedThreadStart(this.StartClient));
                thread1.Start(txtIp.Text); 
            }
        }

        public void ShowWindow()
        {
            this.mainWindow.ShowDialog();
        }

        private void StartClient(object pIPo)
        {
 
            try {
                String dieIp = (String)pIPo;

            this.client.Run(dieIp);
            this.isConnected = true;
            }
            catch(Exception ex)
            {}


        }

        private void StopClient()
        {
            try
            {
                this.isConnected = false;
          
                this.client.Disconnect("Stop Client");

          
            }
            catch (Exception ex)
            { }

        }
    }
}