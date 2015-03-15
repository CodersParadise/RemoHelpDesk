namespace ConsoleClient.Handle
{
    using MarrySocket.MClient;

    public interface IHandlePacket
    {
        void Handle(object receivedClass, ServerSocket serverSocket);
    }
}
