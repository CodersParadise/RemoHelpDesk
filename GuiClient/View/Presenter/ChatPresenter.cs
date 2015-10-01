namespace GuiClient.View.Presenter
{
    using GuiClient.View.Windows;
    using GuiClient.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Input;
    using NAudio.Wave;

    public class ChatPresenter
    {
        private ChatWindow chatWindow;
        private TextBlock textblockChat;
        private TextBox textboxInput;
        private ScrollViewer scrollViewerOutput;
        private List<string> chatHistory;
        private ClientViewModel clientViewModel;
        private Button buttonPushToTalk;
        private WaveIn waveIn;
        private WaveOut waveOut;
        private BufferedWaveProvider waveProvider;

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

            WaveFormat waveFormat = new WaveFormat(8000, 16, 1);
            waveProvider = new BufferedWaveProvider(waveFormat);

            waveOut = new WaveOut();
            waveOut.Init(waveProvider);

            waveIn = new WaveIn();
            waveIn.WaveFormat = waveFormat;

            this.textblockChat = this.chatWindow.textblockChat;
            this.textboxInput = this.chatWindow.textboxInput;
            this.buttonPushToTalk = this.chatWindow.buttonPushToTalk;
            this.scrollViewerOutput = this.chatWindow.scrollViewerOutput;

            this.textboxInput.KeyDown += textboxInput_KeyDown;
            this.buttonPushToTalk.PreviewMouseLeftButtonDown += ButtonPushToTalk_MouseDown;
            this.buttonPushToTalk.PreviewMouseLeftButtonUp += ButtonPushToTalk_MouseUp;
            this.waveIn.DataAvailable += WaveIn_DataAvailable;
            this.clientViewModel.ReceivedVoice += ClientViewModel_ReceivedVoice;
            this.chatWindow.Closed += ChatWindow_Closed;


            Program.DispatchIfNecessary(() =>
            {
                foreach (string chatLine in this.chatHistory)
                {
                    this.textblockChat.Text += chatLine + Environment.NewLine;
                }
            });
        }

        private void ChatWindow_Closed(object sender, EventArgs e)
        {
            this.waveOut.Dispose();
        }

        private void ClientViewModel_ReceivedVoice(byte[] buffer, int length)
        {
            waveProvider.AddSamples(buffer, 0, length);
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            this.clientViewModel.SendVoice(e.Buffer, e.BytesRecorded);
        }

        private void ButtonPushToTalk_MouseUp(object sender, MouseButtonEventArgs e)
        {
            waveIn.StopRecording();
            waveOut.Play();
        }

        private void ButtonPushToTalk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            waveOut.Stop();
            waveIn.StartRecording();
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
