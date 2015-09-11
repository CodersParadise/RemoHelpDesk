namespace GuiClient.ViewModel
{
    using System.Net;
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

        public event CoreClient.DiscoveredServerEventHandler DiscoveredServer
        {
            add { this.coreClient.DiscoveredServer += value; }
            remove { this.coreClient.DiscoveredServer -= value; }
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

        public void SetHost(IPAddress ipAddress, int port)
        {
            this.coreClient.SetHost(ipAddress, port);
        }

        public void SendChat(string message)
        {
            this.coreClient.SendChat(message);
        }


        public void StartDiscover()
        {
            this.coreClient.Discover();
        }
    }

}
