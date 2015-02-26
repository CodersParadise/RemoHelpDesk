namespace ConsoleClient.Handle
{
    using ConsoleClient.Packets;
    using MarrySocket.MClient;

    using System;
    using System.Diagnostics;
    public class HandleRun : IHandlePacket
    {
        public void Handle(object receivedClass, ServerSocket serverSocket)
        {
            String rcmd = (String)receivedClass;
            Process.Start(rcmd);

        }
    }
}
