namespace GuiServer.View.ViewModel
{
    using Arrowgene.Services.Network.ManagedConnection.Client;
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
                    if (cViewModel.Id == clientSocket.Id)
                        clientViewModel = cViewModel;
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
                this.clientViewModels.Add(clientViewModel);
            }
            this.lvClients.ScrollIntoView(clientViewModel);
        }

        public void Remove(ClientViewModel clientViewModel)
        {
            lock (myLock)
            {
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
