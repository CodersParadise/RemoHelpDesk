

namespace GuiServer.View.ViewModel
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Server.Database.Tables;

    public class ChatViewModel
    {
        public enum ChatDirectionType
        {
            Unknown = 0,
            Client = 1,
            Server = 2
        }

        public ChatViewModel(string uniqueHash, ChatDirectionType chatDirection, DateTime timestamp, string message)
        {
            this.UniqueHash = uniqueHash;
            this.ChatDirection = chatDirection;
            this.TimeStamp = timestamp;
            this.Message = message;
        }

        public ChatViewModel(ChatTable chatTable)
        {
            this.UniqueHash = chatTable.UniqueHash;
            this.Message = chatTable.Message;
            this.TimeStamp = chatTable.TimeStamp;
            this.ChatDirection = chatTable.ChatDirection;
        }


        public string UniqueHash { get; set; }
        public ChatDirectionType ChatDirection { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }

    }
}
