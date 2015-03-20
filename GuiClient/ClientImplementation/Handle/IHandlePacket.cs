namespace ConsoleClient.Handle
{
    using GuiClient.ClientImplementation.ViewModel;
    using MarrySocket.MClient;

    public interface IHandlePacket
    {
        void Handle(object receivedClass, ServerSocketViewModel clientViewModel);
    }
}
