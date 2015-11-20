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
        private ObservableCollection<ChatViewModel> chatViewModels;


        public ChatViewModelContainer()
        {
            this.chatViewModels = new ObservableCollection<ChatViewModel>();
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
    }
}
