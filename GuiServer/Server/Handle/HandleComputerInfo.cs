namespace GuiServer.Server.Handle
{
    using GuiServer.Handle;
    using Database;
    using GuiServer.View.ViewModel;
    using NetworkObjects;
    public class HandleComputerInfo : IHandlePacket
    {

        private ClientViewModelContainer clientViewModelContainer;

        public HandleComputerInfo(ClientViewModelContainer clientViewModelContainer)
        {
            this.clientViewModelContainer = clientViewModelContainer;
        }

        public void Handle(object receivedClass, ClientViewModel clientViewModel, Server server)
        {
            ComputerInfo computerInfo = (ComputerInfo)receivedClass;

            if (clientViewModel != null)
            {
                clientViewModel.ComputerInfo = computerInfo;
            }
        }
    }
}
