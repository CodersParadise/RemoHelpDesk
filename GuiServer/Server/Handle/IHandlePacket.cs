namespace GuiServer.Handle
{
    using GuiServer.View.ViewModel;


    public interface IHandlePacket
    {
        void Handle(object receivedClass, ClientViewModel clientViewModel);
    }
}
