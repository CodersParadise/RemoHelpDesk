namespace ClientCore.Packets
{
    using Arrowgene.Services.Network.ManagedConnection.Client;

    public interface ISendPacket
    {
        void Send(ClientSocket clientSocket);
    }
}
