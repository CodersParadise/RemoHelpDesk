namespace GuiServer.Handle
{
    using GuiServer.ServerImplementation.ViewModel;


    public interface IHandlePacket
    {
        void Handle(object receivedClass, ClientViewModel clientViewModel);
    }
}
