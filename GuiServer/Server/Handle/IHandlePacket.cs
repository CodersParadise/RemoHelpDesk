namespace GuiServer.Handle
{
    using GuiServer.View.ViewModel;
    using GuiServer.Server;


    public interface IHandlePacket
    {
        void Handle(object receivedClass, ClientViewModel clientViewModel, Server server);
    }
}
