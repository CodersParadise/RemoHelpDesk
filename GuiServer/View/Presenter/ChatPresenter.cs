namespace GuiServer.View.Presenter
{
    using GuiServer.View.ViewModel;
    using GuiServer.View.Windows;
    using NAudio.Wave;
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
        private WaveIn waveIn;
        private WaveOut waveOut;
        private BufferedWaveProvider waveProvider;
        private Button buttonPushToTalk;

        public ChatPresenter(ClientViewModel clientViewModel)
        {
            this.clientViewModel = clientViewModel;
            this.clientViewModel.CanChat = true;
            this.chatHistory = new List<string>();

            WaveFormat waveFormat = new WaveFormat(8000, 16, 1);
            waveProvider = new BufferedWaveProvider(waveFormat);

            waveOut = new WaveOut();
            waveOut.Init(waveProvider);

            waveIn = new WaveIn();
            waveIn.WaveFormat = waveFormat;
        }

        public void Show()
        {
            if (this.clientViewModel.CanChat)
            {
                this.clientViewModel.CanChat = false;
                this.PrepareChatWindow();
                this.chatWindow.Show();
            }
        }

        private void PrepareChatWindow()
        {
            this.chatWindow = new ChatWindow();
            this.textblockChat = this.chatWindow.textblockChat;
            this.textboxInput = this.chatWindow.textboxInput;
            this.buttonPushToTalk = this.chatWindow.buttonPushToTalk;

            this.chatWindow.Closed += chatWindow_Closed;
            this.textboxInput.KeyDown += textboxInput_KeyDown;
            this.scrollViewerOutput = this.chatWindow.scrollViewerOutput;
            this.chatWindow.Closed += ChatWindow_Closed;
            this.buttonPushToTalk.PreviewMouseLeftButtonDown += ButtonPushToTalk_PreviewMouseLeftButtonDown;
            this.buttonPushToTalk.PreviewMouseLeftButtonUp += ButtonPushToTalk_PreviewMouseLeftButtonUp;
            this.waveIn.DataAvailable += WaveIn_DataAvailable;

            Program.DispatchIfNecessary(() =>
            {
                foreach (string chatLine in this.chatHistory)
                {
                    this.textblockChat.Text += chatLine + Environment.NewLine;
                }
            });
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            this.clientViewModel.SendVoice(e.Buffer, e.BytesRecorded);
        }

        private void ButtonPushToTalk_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            waveIn.StopRecording();
            waveOut.Play();
        }

        private void ButtonPushToTalk_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            waveOut.Stop();
            waveIn.StartRecording();
        }

        private void ChatWindow_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.waveOut.Dispose();
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

        public void PlayVoice(byte[] buffer)
        {
            waveOut.Play();
            waveProvider.AddSamples(buffer, 0, buffer.Length);
        }
    }
}
