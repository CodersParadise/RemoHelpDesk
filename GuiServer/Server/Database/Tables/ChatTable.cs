namespace GuiServer.Server.Database.Tables
{
    using System;
    using View.ViewModel;
    using SQLite;

    public class ChatTable
    {
        public static ChatTable Create(ChatViewModel chatViewModel)
        {
            ChatTable table = new ChatTable();
            table.UniqueHash = chatViewModel.UniqueHash;
            table.TimeStamp = chatViewModel.TimeStamp;
            table.Message = chatViewModel.Message;
            table.ChatDirection = chatViewModel.ChatDirection;
            return table;
        }

        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string UniqueHash { get; set; }
        public ChatViewModel.ChatDirectionType ChatDirection { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }

    }
}
