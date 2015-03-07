namespace ConsoleClient.Handle
{
    using ConsoleClient.Packets;
    using MarrySocket.MClient;

    public class HandleScreenshot : IHandlePacket
    {
        public void Handle(object receivedClass, ServerSocket serverSocket)
        {
            long quality = (long)receivedClass;
            SendScreenshot ss = new SendScreenshot(quality);
            ss.Send(serverSocket);
        }
    }
}
