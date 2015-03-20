using MarrySocket.MClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiClient.ClientImplementation.ViewModel
{
    public class ServerSocketViewModel : INotifyPropertyChanged
    {
        private ServerSocket serverSocketModel;

        public ServerSocketViewModel(ServerSocket serverSocketModel)
        {
            this.serverSocketModel = serverSocketModel;
        }

        public ServerSocket ServerSocket { get { return this.serverSocketModel; } }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
