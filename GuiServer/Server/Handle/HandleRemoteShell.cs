namespace GuiServer.Server.Handle
{
    using GuiServer.Handle;
    using GuiServer.View.ViewModel;
    using System;

    public class HandleRemoteShell : IHandlePacket
    {

        public void Handle(object receivedClass, ClientViewModel clientViewModel, Server server)
        {
            string command = (String)receivedClass;
            clientViewModel.UpdateRemoteShellOutput(command);
        }
    }
}
