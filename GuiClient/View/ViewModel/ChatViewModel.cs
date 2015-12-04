namespace GuiClient.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    public class ChatViewModel
    {
        public static string GetChatDirection(ChatDirectionType type)
        {
            string direction = "-";
            if (type == ChatDirectionType.Client)
            {
                direction = "<=";
            }
            else if (type == ChatDirectionType.Server)
            {
                direction = "=>";
            }
            return direction;
        }


        public enum ChatDirectionType
        {
            Unknown = 0,
            Client = 1,
            Server = 2
        }

        public ChatViewModel(ChatDirectionType chatDirection, DateTime timestamp, string message)
        {
            this.ChatDirection = chatDirection;
            this.TimeStamp = timestamp;
            this.Message = message;
        }

        public ChatDirectionType ChatDirection { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }

    }
}
