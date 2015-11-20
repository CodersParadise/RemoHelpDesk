namespace GuiServer.View.ViewModel
{
    using Arrowgene.Services.Network.ManagedConnection.Client;
    using Server.Database;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;

    public class ClientViewModelContainer
    {
        private ObservableCollection<ClientViewModel> clientViewModels;
        private ListView lvClients;
        private object myLock;

        public ClientViewModelContainer(ListView lvClients)
        {
            this.clientViewModels = new ObservableCollection<ClientViewModel>();
            this.lvClients = lvClients;
            this.myLock = new object();
        }

        public ObservableCollection<ClientViewModel> ClientViewModels { get { return this.clientViewModels; } }

        public ClientViewModel GetClientViewModel(ClientSocket clientSocket)
        {
            ClientViewModel clientViewModel = null;
            lock (myLock)
            {
                foreach (ClientViewModel cViewModel in this.clientViewModels)
                {
                    if (cViewModel.ClientSocket != null && cViewModel.ClientSocket.Id == clientSocket.Id)
                    {
                        clientViewModel = cViewModel;
                    }
                }
            }
            return clientViewModel;
        }

        public ClientViewModel GetClientViewModel(string identityName)
        {
            ClientViewModel clientViewModel = null;
            lock (myLock)
            {
                foreach (ClientViewModel cViewModel in this.clientViewModels)
                {
                    if (cViewModel.IdentityName == identityName)
                        clientViewModel = cViewModel;
                }
            }
            return clientViewModel;
        }

        public void Add(ClientViewModel clientViewModel)
        {
            lock (myLock)
            {
                if (!clientViewModel.IsFromDatabase)
                {
                    // Not from database, check if we have a database model visible, and remove if necessary.
                    ClientViewModel databaseClientViewModel = this.GetClientViewModel(clientViewModel.IdentityName);
                    if (databaseClientViewModel != null)
                    {
                        this.clientViewModels.Remove(databaseClientViewModel);
                    }
                }

                this.clientViewModels.Add(clientViewModel);
                this.lvClients.ScrollIntoView(clientViewModel);
            }

        }

        public void Remove(ClientViewModel clientViewModel)
        {
            lock (myLock)
            {
                if (!clientViewModel.IsFromDatabase)
                {
                    // Not from database, check if we have a database model to display.
                    ClientViewModel databaseClientViewModel = DatabaseManager.Instance.SelectClient(clientViewModel.UniqueHash);
                    if (databaseClientViewModel != null)
                    {
                        this.clientViewModels.Add(databaseClientViewModel);
                    }
                }

                this.clientViewModels.Remove(clientViewModel);
            }
        }

        public void Clear()
        {
            lock (myLock)
            {
                this.clientViewModels.Clear();
            }
        }
    }
}
