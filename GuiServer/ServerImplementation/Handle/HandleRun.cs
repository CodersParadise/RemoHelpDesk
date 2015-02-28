namespace GuiServer.ServerImplementation.Handle
{
    using GuiServer.Handle;
    using GuiServer.ServerImplementation.ViewModel;
    using System;

    public class HandleRun : IHandlePacket
    {

        public void Handle(object receivedClass, ClientViewModel clientViewModel)
        {
            string obj = (String)receivedClass;
            clientViewModel.UpdateRemoteShellOutput(obj);
        }
    }
}
