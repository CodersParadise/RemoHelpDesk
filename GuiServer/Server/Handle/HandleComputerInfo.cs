namespace GuiServer.Server.Handle
{
    using GuiServer.Handle;
    using GuiServer.View.ViewModel;
    using NetworkObjects;
    public class HandleComputerInfo : IHandlePacket
    {

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
