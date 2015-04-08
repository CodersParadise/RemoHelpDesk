namespace GuiServer.Server.Handle
{
    using GuiServer.Handle;
    using GuiServer.View.ViewModel;

    public class HandleChat : IHandlePacket
    {
        public void Handle(object receivedClass, ClientViewModel clientViewModel)
        {
            string message = receivedClass as string;
            clientViewModel.UpdateChat(message);
        }
    }
}
