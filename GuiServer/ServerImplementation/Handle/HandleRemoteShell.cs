namespace GuiServer.ServerImplementation.Handle
{
    using GuiServer.Handle;
    using GuiServer.ServerImplementation.ViewModel;
    using System;

    public class HandleRemoteShell : IHandlePacket
    {

        public void Handle(object receivedClass, ClientViewModel clientViewModel)
        {
            string command = (String)receivedClass;
            clientViewModel.UpdateRemoteShellOutput(command);
        }
    }
}
