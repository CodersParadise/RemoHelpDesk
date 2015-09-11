namespace GuiServer.Server
{
    using Arrowgene.Services.Logging;
    using GuiServer.Handle;
    using GuiServer.Server.Handle;
    using GuiServer.View.ViewModel;
    using NetworkObjects;
    using System;
    using System.Collections.Generic;
    using System.Windows.Threading;

    public class HandlePacket
    {
        private ClientViewModelContainer clientViewModelContainer;
        private Dispatcher dispatcher;
        private Dictionary<int, IHandlePacket> clientPacketIds;
        private Logger logger;
        private Server server;

        public HandlePacket(ClientViewModelContainer clientViewModelContainer, Dispatcher dispatcher, Logger logger, Server server)
        {
            this.server = server;
            this.clientViewModelContainer = clientViewModelContainer;
            this.dispatcher = dispatcher;
            this.clientPacketIds = new Dictionary<int, IHandlePacket>();
            this.logger = logger;
            this.InitPacketIds();
        }

        private void InitPacketIds()
        {
            this.clientPacketIds.Add(PacketId.COMPUTER_INFO, new HandleComputerInfo());
            this.clientPacketIds.Add(PacketId.SCREEN_SHOT, new HandleScreenshot());
            this.clientPacketIds.Add(PacketId.RUN, new HandleRemoteShell());
            this.clientPacketIds.Add(PacketId.CHAT, new HandleChat());
       
        }

        public void Handle(int packetId, object receivedClass, ClientViewModel clientViewModel)
        {
            if (clientPacketIds.ContainsKey(packetId))
            {
                if (clientViewModel != null)
                {
                    clientViewModel.NotifyInTrafficChanged();
                    this.clientPacketIds[packetId].Handle(receivedClass, clientViewModel, this.server);
                }
                else
                {
                    logger.Write(String.Format("Could not handle packet: {0} (ViewModel is Null)", packetId), LogType.PACKET);
                }
            }
            else
            {
                logger.Write(String.Format("Could not handle packet: {0}", packetId), LogType.PACKET);
            }
        }

    }
}


