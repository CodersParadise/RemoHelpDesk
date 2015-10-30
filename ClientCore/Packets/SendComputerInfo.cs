namespace ClientCore.Packets
{
    using Arrowgene.Services.Common;
    using Arrowgene.Services.Network.ManagedConnection.Client;
    using NetworkObjects;
    using System;
    using System.Security.Principal;

    public class SendComputerInfo : ISendPacket
    {

        public void Send(ClientSocket serverSocket)
        {

            ComputerInfo computerInfo = new ComputerInfo(System.Environment.MachineName);
            computerInfo.Device = Environment.OSVersion.ToString();
            computerInfo.LogonName = Environment.UserName.ToString();
            computerInfo.OsVersion = (int)OS.GetOperatingSystemVersion();


            WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            computerInfo.IdentityName = identity.Name;


            serverSocket.SendObject(PacketId.COMPUTER_INFO, computerInfo);
        }
    }
}
