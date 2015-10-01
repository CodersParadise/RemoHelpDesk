namespace GuiServer.Server.Handle
{
    using GuiServer.Handle;
    using GuiServer.View.ViewModel;

    public class HandleVoice : IHandlePacket
    {
        public void Handle(object receivedClass, ClientViewModel clientViewModel, Server server)
        {
            byte[] buffer = receivedClass as byte[];
            clientViewModel.PlayVoice(buffer);
            server.RaiseDisplayTrayBalloon("New Voice from:", clientViewModel.UniqueId.ToString());
        }
    }
}
