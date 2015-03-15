namespace ConsoleClient.Packets
{
    using MarrySocket.MClient;
    using MarrySocket.MExtra;
    using NetworkObjects;
    using System;

    public class SendComputerInfo : ISendPacket
    {

        public void Send(ServerSocket serverSocket)
        {
            ComputerInfo computerInfo = new ComputerInfo(System.Environment.MachineName);
            computerInfo.Device = Environment.OSVersion.ToString();
            computerInfo.LogonName = Environment.UserName.ToString();
            computerInfo.OsVersion = (int)Maid.GetOperatingSystemVersion();
            //serverSocket.SendObject(PacketId.COMPUTER_INFO, computerInfo);
            serverSocket.SendObject(PacketId.COMPUTER_INFO, computerInfo);
        }
    }
}
