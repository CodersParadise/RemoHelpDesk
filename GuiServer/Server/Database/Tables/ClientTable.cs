namespace GuiServer.Server.Database.Tables
{
    using View.ViewModel;
    using SQLite;
    using System;

    public class ClientTable
    {
        public static ClientTable Create(ClientViewModel clientViewModel)
        {
            ClientTable table = new ClientTable();
            table.Id = clientViewModel.Id;
            table.Ip = clientViewModel.Ip;
            table.InTraffic = clientViewModel.ClientSocket.InTraffic;
            table.OutTraffic = clientViewModel.ClientSocket.OutTraffic;
            table.Device = clientViewModel.Device;
            table.LogonName = clientViewModel.LogonName;
            table.HostName = clientViewModel.HostName;
            table.IdentityName = clientViewModel.IdentityName;
            return table;
        }


        [PrimaryKey MaxLength(36)]
        public string IdentityName { get; internal set; }
        public int Id { get; set; }
        public string Ip { get; set; }
        public long InTraffic { get; set; }
        public long OutTraffic { get; set; }
        public string Device { get; internal set; }
        public string LogonName { get; internal set; }
        public string HostName { get; internal set; }

    }
}
