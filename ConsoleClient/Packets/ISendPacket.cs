namespace ClientCore.Packets
{
    using MarrySocket.MClient;

    public interface ISendPacket
    {
        void Send(ServerSocket serverSocket);
    }
}
