namespace GuiServer.View.ViewModel
{
    using MarrySocket.MServer;
    using System.Collections.ObjectModel;
using System.Windows.Controls;

    public class ClientViewModelContainer
    {
        private ObservableCollection<ClientViewModel> clientViewModels;
        private ListView lvClients;

        public ClientViewModelContainer(ListView lvClients)
        {
            this.clientViewModels = new ObservableCollection<ClientViewModel>();
            this.lvClients = lvClients;
        }

        public ObservableCollection<ClientViewModel> ClientViewModels { get { return this.clientViewModels; } }

        public ClientViewModel GetClientViewModel(ClientSocket clientSocket)
        {
            ClientViewModel clientViewModel = null;
            foreach (ClientViewModel cViewModel in this.clientViewModels)
            {
                if (cViewModel.Id == clientSocket.Id)
                    clientViewModel = cViewModel;
            }
            return clientViewModel;
        }

        public void Add(ClientViewModel clientViewModel)
        {
            this.clientViewModels.Add(clientViewModel);
            this.lvClients.ScrollIntoView(clientViewModel);
        }

        public void Remove(ClientViewModel clientViewModel)
        {
            this.clientViewModels.Remove(clientViewModel);
        }

        public void Clear()
        {
            this.clientViewModels.Clear();
        }
    }
}
