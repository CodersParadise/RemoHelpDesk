namespace GuiClient.ViewModel
{
    using ClientCore;
    using System.Net;

    public class ClientViewModel
    {
        private CoreClient coreClient;

        public ClientViewModel()
        {
            this.coreClient = new CoreClient();
            this.coreClient.Init();
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

    }

}
