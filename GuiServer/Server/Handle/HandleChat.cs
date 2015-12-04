namespace GuiServer.Server.Handle
{
    using System;
    using GuiServer.Handle;
    using GuiServer.View.ViewModel;

    public class HandleChat : IHandlePacket
    {
        private ChatViewModelContainer chatViewModelContainer;

        public HandleChat(ChatViewModelContainer chatViewModelContainer)
        {
            this.chatViewModelContainer = chatViewModelContainer;
        }


        public void Handle(object receivedClass, ClientViewModel clientViewModel, Server server)
        {
            string message = receivedClass as string;

            ChatViewModel chatViewModel = new ChatViewModel(clientViewModel.UniqueHash, ChatViewModel.ChatDirectionType.Client, DateTime.Now, message);
            this.chatViewModelContainer.Add(chatViewModel);

            clientViewModel.UpdateChat(chatViewModel);
            server.RaiseDisplayTrayBalloon("New Message", message);
        }
    }
}
