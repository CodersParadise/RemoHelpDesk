namespace ClientCore.Packets
{
    using Arrowgene.Services.Network.MarrySocket.MClient;


    public interface ISendPacket
    {
        void Send(ServerSocket serverSocket);
    }
}
