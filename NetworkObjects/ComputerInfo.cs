﻿namespace NetworkObjects
{
    using System;

    [Serializable]
    public class ComputerInfo
    {
        public ComputerInfo(string hostName)
        {
            this.HostName = hostName;
        }

        public string Device { get; set; }
        public string HostName { get; set; }
        public string LogonName { get; set; }
        public int LogonCount { get; set; }
        public int OsVersion { get; set; }
        public string IdentityName { get; set; }
        public string MacAddress { get; set; }
    }
}
