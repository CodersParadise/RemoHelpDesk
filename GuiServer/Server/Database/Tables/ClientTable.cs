namespace GuiServer.Server.Database.Tables
{
    using View.ViewModel;
    using SQLite;
    using System;
    using Arrowgene.Services.Common;

    public class ClientTable
    {
        public static ClientTable Create(ClientViewModel clientViewModel)
        {
            ClientTable table = new ClientTable();
            table.Ip = clientViewModel.Ip;
            table.Device = clientViewModel.Device;
            table.LogonName = clientViewModel.LogonName;
            table.HostName = clientViewModel.HostName;
            table.IdentityName = clientViewModel.IdentityName;
            table.MacAddress = clientViewModel.MacAddress;
            table.UniqueHash = clientViewModel.UniqueHash;
            table.OsVersion = clientViewModel.OsVersion;
            return table;
        }


        [PrimaryKey MaxLength(32)]
        public string UniqueHash { get; set; }
        public string IdentityName { get; set; }
        public string Ip { get; set; }
        public long InTraffic { get; set; }
        public long OutTraffic { get; set; }
        public string Device { get; set; }
        public string LogonName { get; set; }
        public string HostName { get; set; }
        public string MacAddress { get; set; }
        public OS.OsVersion OsVersion { get; set; }
    }
}
