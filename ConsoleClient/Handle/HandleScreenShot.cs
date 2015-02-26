namespace ConsoleClient.Handle
{
    using ConsoleClient.Packets;
    using MarrySocket.MClient;

    public class HandleScreenShot : IHandlePacket
    {
        public void Handle(object receivedClass, ServerSocket serverSocket)
        {
            long quality = (long)receivedClass;
            SendScreenShot ss = new SendScreenShot(quality);
            ss.Send(serverSocket);
        }
    }
}
