namespace ClientCore.Handle
{
    using Arrowgene.Services.Network.MarrySocket.MClient;


    public interface IHandlePacket
    {
        void Handle(object receivedClass, ServerSocket serverSocket);
    }
}
