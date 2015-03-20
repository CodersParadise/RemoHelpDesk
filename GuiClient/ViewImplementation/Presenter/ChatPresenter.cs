using GuiClient.ClientImplementation;
using GuiClient.ViewImplementation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GuiClient.ViewImplementation.Presenter
{
    public class ChatPresenter
    {

        private ChatWindow chatWindow;
        private TextBlock textblockChat;
        private TextBox textboxInput;
        private Client derClient;

        private ScrollViewer scrollViewerOutput;


        public ChatPresenter(ChatWindow chatWindow, Client derClient)
        {
            this.chatWindow = chatWindow;
            this.AssignFields();
            this.InitializeViewEvents();
            this.derClient = derClient;

        }

        private void AssignFields()
        {
            this.textblockChat = this.chatWindow.textblockChat;
            this.textboxInput = this.chatWindow.textboxInput;


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


        private void InitializeViewEvents()
        {
            this.chatWindow.Closed += chatWindow_Closed;

        }


        void chatWindow_Closed(object sender, System.EventArgs e)
        {
        }


        public void ShowWindow()
        {
            this.chatWindow.ShowDialog();
        }


    }


}
