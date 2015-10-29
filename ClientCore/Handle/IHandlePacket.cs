namespace ClientCore.Handle
{
    using Arrowgene.Services.Network.ManagedConnection.Client;



    public interface IHandlePacket
    {
        void Handle(object receivedClass, ClientSocket clientSocket);
    }
}
