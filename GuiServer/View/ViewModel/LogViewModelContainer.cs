namespace GuiServer.View.ViewModel
{
    using MarrySocket.MServer;
    using System.Collections.ObjectModel;
using System.Windows.Controls;

    public class LogViewModelContainer
    {
        private ObservableCollection<LogViewModel> logViewModels;
        private ListView lvLogs;

        public LogViewModelContainer(ListView lvLogs)
        {
            this.logViewModels = new ObservableCollection<LogViewModel>();
            this.lvLogs = lvLogs;
        }

        public ObservableCollection<LogViewModel> LogViewModels { get { return this.logViewModels; } }


        public void Add(LogViewModel logViewModel)
        {
            this.logViewModels.Add(logViewModel);
            this.lvLogs.ScrollIntoView(logViewModel);
        }

        public void Remove(LogViewModel logViewModel)
        {
            this.logViewModels.Remove(logViewModel);
        }

        public void Clear()
        {
            this.logViewModels.Clear();
        }
    }
}
