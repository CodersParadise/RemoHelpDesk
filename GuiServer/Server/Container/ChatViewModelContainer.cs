namespace GuiServer.View.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Server.Database;

    public class ChatViewModelContainer
    {
        private static ChatViewModelContainer _chatViewModelContainer;

        public static ChatViewModelContainer Instance
        {
            get
            {
                if (_chatViewModelContainer == null)
                {
                    _chatViewModelContainer = new ChatViewModelContainer();
                }
                return _chatViewModelContainer;
            }
        }


        private ObservableCollection<ChatViewModel> chatViewModels;


        private ChatViewModelContainer()
        {
            this.chatViewModels = new ObservableCollection<ChatViewModel>();

            foreach (ChatViewModel chatViewModel in DatabaseManager.Instance.SelectAllChats())
            {
                this.chatViewModels.Add(chatViewModel);
            }
        }

        public ObservableCollection<ChatViewModel> ChatViewModels { get { return this.chatViewModels; } }


        public void Add(ChatViewModel chatViewModel)
        {
            DatabaseManager.Instance.InsertChat(chatViewModel);
            this.ChatViewModels.Add(chatViewModel);
        }

        public void Remove(ChatViewModel chatViewModel)
        {
            this.ChatViewModels.Remove(chatViewModel);
        }

        public void Clear()
        {
            this.ChatViewModels.Clear();
        }

        public List<ChatViewModel> GetChatViewModels(string uniqueHash)
        {
            List<ChatViewModel> chatViewModels = new List<ChatViewModel>();

            foreach (ChatViewModel chatViewModel in this.chatViewModels)
            {
                if (chatViewModel.UniqueHash == uniqueHash)
                {
                    chatViewModels.Add(chatViewModel);
                }
            }

            return chatViewModels;
        }

    }
}
