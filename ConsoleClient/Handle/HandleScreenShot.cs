namespace ConsoleClient.Handle
{
    using ConsoleClient.Packets;
    using ConsoleClient.Tools;
    using MarrySocket.MClient;


    public class HandleScreenshot : IHandlePacket
    {
        public void Handle(object receivedClass, ServerSocket serverSocket)
        {
            if (receivedClass is long)
            {
                long quality = (long)receivedClass;
                SendScreenshot ss = new SendScreenshot(quality);
                ss.Send(serverSocket);
            }
            else if (receivedClass is int[])
            {
                int[] position = (int[])receivedClass;
                InputController.ClickOnPoint(position[0], position[1]);
            }
        }
    }
}
