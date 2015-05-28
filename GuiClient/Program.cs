namespace GuiClient
{
    using GuiClient.View.Presenter;
    using GuiClient.View.Windows;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public static class Program
    {
        private static MainWindow mainWindow;
        private const string assemblyClientCore = "ClientCore";

        [STAThreadAttribute()]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Run();
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

        
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            if (args.Name.Contains(assemblyClientCore))
            {
                assembly = LoadAssembly(assemblyClientCore);
            }
            else
            {
                Debug.WriteLine("Missing Assembly:" + args.Name);
            }
            return assembly;
        }

        private static Assembly LoadAssembly(string name)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClientCore." + name + ".dll"))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }



    }



