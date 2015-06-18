namespace GuiServer.Server.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DisplayTrayBalloonEventArgs : EventArgs
    {
        public DisplayTrayBalloonEventArgs(string title, string text)
        {
            this.Text = text;
            this.Title = title;

        }

        public string Title { get; private set; }
        public string Text { get; private set; }
    }
}
