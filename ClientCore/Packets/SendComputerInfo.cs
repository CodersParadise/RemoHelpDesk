namespace ClientCore.Packets
{
    using Arrowgene.Services.Common;
    using Arrowgene.Services.Network.ManagedConnection.Client;
    using NetworkObjects;
    using System;

    public class SendComputerInfo : ISendPacket
    {

        public void Send(ClientSocket serverSocket)
        {
            ComputerInfo computerInfo = new ComputerInfo(System.Environment.MachineName);
            computerInfo.Device = Environment.OSVersion.ToString();
            computerInfo.LogonName = Environment.UserName.ToString();
            computerInfo.OsVersion = (int)OS.GetOperatingSystemVersion();
            serverSocket.SendObject(PacketId.COMPUTER_INFO, computerInfo);
        }
    }
}
