namespace GuiServer
{
    using GuiServer.ViewImplementation;
    using System;

    public static class Program
    {
        private static MainWindow mainWindow;

        [STAThreadAttribute()]
        public static void Main()
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
