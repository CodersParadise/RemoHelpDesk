namespace GuiClient
{
    using GuiClient.View.Presenter;
    using GuiClient.View.Windows;
    using System;
    using System.IO;
    using System.Reflection;


    public static class Program
    {
        private static MainWindow mainWindow;

        [STAThreadAttribute()]
        public static void Main()
        {
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

    }
}


