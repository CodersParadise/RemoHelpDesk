namespace GuiServer.ServerImplementation.Handle
{
    using GuiServer.Handle;
    using GuiServer.ServerImplementation.ViewModel;
    using NetworkObjects;
    public class HandleComputerInfo : IHandlePacket
    {

        public void Handle(object receivedClass, ClientViewModel clientViewModel)
        {
            ComputerInfo computerInfo = (ComputerInfo)receivedClass;

            if (clientViewModel != null)
            {
                clientViewModel.ComputerInfo = computerInfo;
            }
        }
    }
}
