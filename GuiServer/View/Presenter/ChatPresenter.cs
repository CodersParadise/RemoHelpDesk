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
    using System.Windows;

    public class ChatPresenter
    {
        private ChatWindow chatWindow;
        private ClientViewModel clientViewModel;
        private TextBox textboxInput;
        private TextBlock textblockChat;
        private ScrollViewer scrollViewerOutput;
        private List<ChatViewModel> chatHistory;
        private WaveIn waveIn;
        private WaveOut waveOut;
        private BufferedWaveProvider waveProvider;
        private Button buttonPushToTalk;
        private CheckBox checkBoxVoiceActive;
        private ChatViewModelContainer chatViewModelContainer;


        public ChatPresenter(ClientViewModel clientViewModel)
        {
            this.chatViewModelContainer = clientViewModel.ChatViewModelContainer;
            this.clientViewModel = clientViewModel;
            this.clientViewModel.CanChat = true;

            WaveFormat waveFormat = new WaveFormat(8000, 16, 1);
            waveProvider = new BufferedWaveProvider(waveFormat);

            if (!clientViewModel.IsFromDatabase)
            {
                waveOut = new WaveOut();
                waveOut.Init(waveProvider);

                waveIn = new WaveIn();
                waveIn.WaveFormat = waveFormat;

                this.waveIn.DataAvailable += WaveIn_DataAvailable;
            }
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
            this.checkBoxVoiceActive = this.chatWindow.checkBoxActive;

            this.buttonPushToTalk.IsEnabled = false;
            this.chatWindow.Closed += chatWindow_Closed;
            this.textboxInput.KeyDown += textboxInput_KeyDown;
            this.scrollViewerOutput = this.chatWindow.scrollViewerOutput;
            this.chatWindow.Closed += ChatWindow_Closed;
            this.buttonPushToTalk.PreviewMouseLeftButtonDown += ButtonPushToTalk_PreviewMouseLeftButtonDown;
            this.buttonPushToTalk.PreviewMouseLeftButtonUp += ButtonPushToTalk_PreviewMouseLeftButtonUp;

            this.checkBoxVoiceActive.Checked += CheckBoxVoiceActive_Checked;
            this.checkBoxVoiceActive.Unchecked += CheckBoxVoiceActive_Checked;


            this.textboxInput.Focus();

            this.chatHistory = this.chatViewModelContainer.GetChatViewModels(this.clientViewModel.UniqueHash);

            Program.DispatchIfNecessary(() =>
            {
                this.textblockChat.Text = string.Empty;
                foreach (ChatViewModel chatViewModel in this.chatHistory)
                {
                    this.Update(chatViewModel);
                }
            });
        }

        private void CheckBoxVoiceActive_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.checkBoxVoiceActive.IsChecked == true)
            {
                this.buttonPushToTalk.IsEnabled = true;
                waveOut.Play();
            }
            else
            {
                this.buttonPushToTalk.IsEnabled = false;
                waveOut.Stop();
            }
        }


        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            this.clientViewModel.SendVoice(e.Buffer, e.BytesRecorded);
        }

        private void ButtonPushToTalk_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            waveIn.StopRecording();
        }

        private void ButtonPushToTalk_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                waveIn.StartRecording();
            }
            catch (NAudio.MmException ex)
            {
                string message = ex.Message;
                if (ex.Result == NAudio.MmResult.BadDeviceId)
                {
                    message = "Microphone not found";
                }
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChatWindow_Closed(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (waveOut != null)
            {
                this.waveOut.Dispose();
            }

        }

        public void Update(ChatViewModel message)
        {
            if (this.chatWindow != null && message != null)
            {
                Program.DispatchIfNecessary(() =>
                     {
                         this.textblockChat.Text += string.Format("{0} {1} {2}", ChatViewModel.GetChatDirection(message.ChatDirection), message.Message, Environment.NewLine);
                         this.scrollViewerOutput.ScrollToBottom();
                     });
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
            if (this.clientViewModel.IsFromDatabase)
            {
                Program.DispatchIfNecessary(() =>
                {
                    this.textboxInput.Text = string.Empty;
                    MessageBox.Show("Nicht verbunden!", "Der Client ist nicht verbunden, das Senden ist nicht möglich.", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            else
            {
                ChatViewModel chatViewModel = null;
                string input = this.textboxInput.Text;
                if (!string.IsNullOrEmpty(input) && input.Length > 0)
                {

                    this.clientViewModel.SendObject(PacketId.CHAT, input);

                    chatViewModel = new ChatViewModel(this.clientViewModel.UniqueHash, ChatViewModel.ChatDirectionType.Server, DateTime.Now, input);
                    this.chatViewModelContainer.Add(chatViewModel);

                    Program.DispatchIfNecessary(() =>
                    {
                        this.textboxInput.Text = string.Empty;
                    });
                }

                this.Update(chatViewModel);
            }


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
