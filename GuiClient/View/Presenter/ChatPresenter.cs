namespace GuiClient.View.Presenter
{
    using GuiClient.View.Windows;
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class ChatPresenter
    {
        private ChatWindow chatWindow;
        private TextBlock textblockChat;
        private TextBox textboxInput;
        private ScrollViewer scrollViewerOutput;

        public ChatPresenter(ChatWindow chatWindow)
        {
            this.chatWindow = chatWindow;
            this.AssignControls();
            this.AssignEvents();
            this.PrepareContent();
        }

        private void AssignControls()
        {
            this.textblockChat = this.chatWindow.textblockChat;
            this.textboxInput = this.chatWindow.textboxInput;
            this.scrollViewerOutput = this.chatWindow.scrollViewerOutput;
        }
        private void AssignEvents()
        {
            this.chatWindow.Closed += chatWindow_Closed;
        }

        private void PrepareContent()
        {


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
                //   this.ClientViewModel.derClient.SendObject(PacketId.RUN, input);
                this.textboxInput.Text = string.Empty;
            }
        }

        private void remoteShellWindow_Closed(object sender, System.EventArgs e)
        {
            //    this.clientViewModel.SendObject(PacketId.RUN, "#close");
            //   this.clientViewModel.CanRemoteShell = true;
        }

        public void Show()
        {
            // if (this.clientViewModel.CanRemoteShell)
            //{
            //  this.clientViewModel.CanRemoteShell = false;
            this.chatWindow.Show();
            // this.clientViewModel.SendObject(PacketId.RUN, "#init");
            //   }
        }

        public void UpdateOutput(string output)
        {
            Program.DispatchIfNecessary(() =>
            {
                this.textblockChat.Text += output + Environment.NewLine;
                this.scrollViewerOutput.ScrollToBottom();
            });


        }




        private void chatWindow_Closed(object sender, System.EventArgs e)
        {
        }


        public void ShowWindow()
        {
            this.chatWindow.ShowDialog();
        }


    }


}
