﻿namespace GuiServer
{
    using GuiServer.View;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public static class Program
    {
        private static MainWindow mainWindow;
        private const string assemblyMarrySocket = "Arrowgene.Services";
        private const string assemblyNetworkObjects = "NetworkObjects";
        public const string PROGRAMM_NAME = "RemoHelpDesk";
        public const string assemblyNAudio = "NAudio";

        public const string deploy_AssemblySqlLite = "sqlite3";

        [STAThreadAttribute()]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Run();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            if (args.Name.Contains(assemblyMarrySocket))
            {
                assembly = LoadAssembly(assemblyMarrySocket);
            }
            else if (args.Name.Contains(assemblyNetworkObjects))
            {
                assembly = LoadAssembly(assemblyNetworkObjects);
            }
            else if (args.Name.Contains(assemblyNAudio))
            {
                assembly = LoadAssembly(assemblyNAudio);
            }
            else
            {
                Debug.WriteLine("Missing Assembly:" + args.Name);
            }
            return assembly;
        }

        private static void DeploySqLite()
        {
            if (!File.Exists(deploy_AssemblySqlLite + ".dll"))
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GuiServer." + deploy_AssemblySqlLite + ".dll"))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);

                    using (Stream file = File.Create(deploy_AssemblySqlLite + ".dll"))
                    {
                        file.Write(assemblyData, 0, assemblyData.Length);
                    }
                }
            }
        }

        private static Assembly LoadAssembly(string name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GuiServer." + name + ".dll"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        private static void Run()
        {
            DeploySqLite();

            mainWindow = new MainWindow();
            MainPresenter mainPresenter = new MainPresenter(mainWindow);
            mainPresenter.ShowWindow();
        }

        public static void DispatchIfNecessary(Action action)
        {
            if (!mainWindow.Dispatcher.CheckAccess())
                mainWindow.Dispatcher.Invoke(action);
            else
                action.Invoke();
        }

        public static void Dispatch(Action action)
        {
            mainWindow.Dispatcher.Invoke(action);
        }

        public static string GetApplicationPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }



    }
}
