﻿namespace GuiServer
{
    using GuiServer.ViewImplementation;
    using System;
    using System.IO;
    using System.Reflection;

    public static class Program
    {
        private static MainWindow mainWindow;
        private const string assemblyMarrySocket = "MarrySocket";
        private const string assemblyNetworkObjects = "NetworkObjects";

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
            else
            {
                Console.WriteLine("Missing Assembly:" + args.Name);
                Console.ReadKey();
                Environment.Exit(0);
            }
            return assembly;
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

    }
}
