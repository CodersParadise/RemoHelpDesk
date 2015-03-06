namespace ConsoleClient.Packets
{
    using MarrySocket.MClient;
    using NetworkObjects;
    using System;
    
    public class SendComputerInfo : ISendPacket
    {

        public void Send(ServerSocket serverSocket)
        {
            ComputerInfo computerInfo = new ComputerInfo(System.Environment.MachineName);
            computerInfo.Device = Environment.OSVersion.ToString();
            computerInfo.LogonName = Environment.UserName.ToString();

            //serverSocket.SendObject(PacketId.COMPUTER_INFO, computerInfo);
            serverSocket.SendObject(PacketId.COMPUTER_INFO, computerInfo);
        }
    }
}
