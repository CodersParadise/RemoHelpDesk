namespace GuiClient.ViewModel
{
    using ClientCore;
    using GuiClient.View.Presenter;

    public class ClientViewModel
    {
        private CoreClient coreClient;
        private ChatPresenter chatPresenter;


        public ClientViewModel()
        {
            this.coreClient = new CoreClient();
            this.coreClient.Init();
            this.coreClient.ReceivedChat += coreClient_ReceivedChat;
        }

        private void coreClient_ReceivedChat(string message)
        {
            if (this.chatPresenter != null)
            {
                this.chatPresenter.Update(message);
            }
        }

        public void SetChatPresenter(ChatPresenter chatPresenter)
        {
            this.chatPresenter = chatPresenter;
        }

        public bool IsConnected { get { return this.coreClient.IsConnected; } }

        public void Connect()
        {
            this.coreClient.Connect();
        }

        public void Disconnect()
        {
            this.coreClient.Disconnect();
        }

        public void SetHost(string ipAddress, int port)
        {
            this.coreClient.SetHost(ipAddress, port);
        }

        public void SendChat(string message)
        {
            this.coreClient.SendChat(message);
        }

    }

}
