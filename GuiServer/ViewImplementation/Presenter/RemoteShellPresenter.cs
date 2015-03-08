namespace GuiServer.ViewImplementation.Presenter
{
    using GuiServer.ServerImplementation.ViewModel;
    using GuiServer.ViewImplementation.Windows;
    using NetworkObjects;
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class RemoteShellPresenter
    {
        private RemoteShellWindow remoteShellWindow;
        private ClientViewModel clientViewModel;
        private TextBox textboxInput;
        private TextBlock textblockOutput;
        private ScrollViewer scrollViewerOutput;

        public RemoteShellPresenter(ClientViewModel clientViewModel)
        {
            this.clientViewModel = clientViewModel;
            this.clientViewModel.CanRemoteShell = true;
        }

        public void SetRemoteShellWindow(RemoteShellWindow remoteShellWindow)
        {
            if (remoteShellWindow != null && this.clientViewModel.CanRemoteShell)
            {
                this.remoteShellWindow = remoteShellWindow;
                this.remoteShellWindow.Title = this.clientViewModel.FullName;
                this.remoteShellWindow.Closing += remoteShellWindow_Closed;
                this.remoteShellWindow.Closed += remoteShellWindow_Closed;
                this.scrollViewerOutput = this.remoteShellWindow.scrollViewerOutput;
                this.textblockOutput = this.remoteShellWindow.textblockOutput;
                this.textboxInput = this.remoteShellWindow.textboxInput;
                this.textboxInput.KeyDown += textboxInput_KeyDown;
            }

        }

        private void textboxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.AcceptInput();
            }
        }

        private void AcceptInput()
        {
            string input = this.textboxInput.Text;
            if (!string.IsNullOrEmpty(input) && input.Length > 0)
            {
                this.clientViewModel.SendObject(PacketId.RUN, input);
                this.textboxInput.Text = string.Empty;
            }
        }

        private void remoteShellWindow_Closed(object sender, System.EventArgs e)
        {
            this.clientViewModel.SendObject(PacketId.RUN, "#close");
            this.clientViewModel.CanRemoteShell = true;
        }

        public void Show()
        {
            if (this.clientViewModel.CanRemoteShell)
            {
                this.clientViewModel.CanRemoteShell = false;
                this.remoteShellWindow.Show();
                this.clientViewModel.SendObject(PacketId.RUN, "#init");
            }
        }

        public void UpdateOutput(string output)
        {
            Program.DispatchIfNecessary(() =>
            {
                this.textblockOutput.Text += output + Environment.NewLine;
                this.scrollViewerOutput.ScrollToBottom();
            });


        }

    }
}
