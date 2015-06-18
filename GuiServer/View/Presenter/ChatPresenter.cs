namespace GuiServer.View.Presenter
{
    using GuiServer.View.ViewModel;
    using GuiServer.View.Windows;
    using NetworkObjects;
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class ChatPresenter
    {
        private ChatWindow chatWindow;
        private ClientViewModel clientViewModel;
        private TextBox textboxInput;
        private TextBlock textblockChat;
        private ScrollViewer scrollViewerOutput;
        private List<string> chatHistory;

        public ChatPresenter(ClientViewModel clientViewModel)
        {
            this.clientViewModel = clientViewModel;
            this.clientViewModel.CanChat = true;
            this.chatHistory = new List<string>();
        }

        public void Show()
        {
            if (this.clientViewModel.CanRemoteShell)
            {
                this.clientViewModel.CanChat = false;
                this.PrepareChatWindow();
                this.chatWindow.Show();
            }
        }

        private void PrepareChatWindow()
        {
            this.chatWindow = new ChatWindow();
            this.chatWindow.Closed += chatWindow_Closed;
            this.textblockChat = this.chatWindow.textblockChat;
            this.textboxInput = this.chatWindow.textboxInput;
            this.textboxInput.KeyDown += textboxInput_KeyDown;
            this.scrollViewerOutput = this.chatWindow.scrollViewerOutput;

            Program.DispatchIfNecessary(() =>
            {
                foreach (string chatLine in this.chatHistory)
                {
                    this.textblockChat.Text += chatLine + Environment.NewLine;
                }
            });
        }

        public void Update(string message)
        {
            if (this.chatWindow != null)
            {
                Program.DispatchIfNecessary(() =>
                     {
                         this.textblockChat.Text += message + Environment.NewLine;
                         this.scrollViewerOutput.ScrollToBottom();
                     });
            }

            this.chatHistory.Add(message);
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
                this.clientViewModel.SendObject(PacketId.CHAT, input);

                Program.DispatchIfNecessary(() =>
                {
                    this.textboxInput.Text = string.Empty;
                });
            }

            this.Update(input);
        }

        private void chatWindow_Closed(object sender, EventArgs e)
        {
            this.clientViewModel.CanChat = true;
        }

    }
}
