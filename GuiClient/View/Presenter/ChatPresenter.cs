namespace GuiClient.View.Presenter
{
    using GuiClient.View.Windows;
    using GuiClient.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class ChatPresenter
    {
        private ChatWindow chatWindow;
        private TextBlock textblockChat;
        private TextBox textboxInput;
        private ScrollViewer scrollViewerOutput;
        private List<string> chatHistory;
        private ClientViewModel clientViewModel;

        public ChatPresenter()
        {
            this.chatHistory = new List<string>();
        }
        public void Show(ClientViewModel clientViewModel)
        {
            this.clientViewModel = clientViewModel;
            this.PrepareChatWindow();
            this.chatWindow.ShowDialog();
        }

        private void PrepareChatWindow()
        {
            this.chatWindow = new ChatWindow();

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

            if (clientViewModel.IsConnected)
            {

                if (!string.IsNullOrEmpty(input) && input.Length > 0)
                {
                    Program.DispatchIfNecessary(() =>
                    {
                        this.textboxInput.Text = string.Empty;
                    });

                    this.Update(input);
                    this.clientViewModel.SendChat(input);
                }
            }
            else
            {
                Program.DispatchIfNecessary(() =>
                {
                    this.textboxInput.Text = string.Empty;
                });
                this.Update("Sie müssen sich erst mit einem Supportmitarbeiter verbinden.");
            }
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

    }
}
