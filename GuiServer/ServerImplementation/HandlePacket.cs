namespace GuiServer.ServerImplementation
{

    using GuiServer.Handle;
    using GuiServer.ServerImplementation.Handle;
    using GuiServer.ServerImplementation.ViewModel;
    using MarrySocket.MExtra.Logging;
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

        public HandlePacket(ClientViewModelContainer clientViewModelContainer, Dispatcher dispatcher, Logger logger)
        {
            this.clientViewModelContainer = clientViewModelContainer;
            this.dispatcher = dispatcher;
            this.clientPacketIds = new Dictionary<int, IHandlePacket>();
            this.logger = logger;
            this.InitPacketIds();
        }

        private void InitPacketIds()
        {
            this.clientPacketIds.Add(PacketId.COMPUTER_INFO, new HandleComputerInfo());
            this.clientPacketIds.Add(PacketId.SCREEN_SHOT, new HandleScreenShot());
            this.clientPacketIds.Add(PacketId.RUN, new HandleRun());
        }

        public void Handle(int packetId, object receivedClass, ClientViewModel clientViewModel)
        {
            if (clientPacketIds.ContainsKey(packetId))
            {
                this.clientPacketIds[packetId].Handle(receivedClass, clientViewModel);
            }
            else
            {
                logger.Write(String.Format("Could not handle packet: {0}", packetId), LogType.PACKET);
            }
        }

    }
}


