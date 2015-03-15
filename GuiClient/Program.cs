using GuiClient.ViewImplementation;
using GuiClient.ViewImplementation.Presenter;

using System;
using System.IO;
using System.Reflection;


namespace GuiClient
{
 

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


