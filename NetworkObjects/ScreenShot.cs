namespace NetworkObjects
{
    using System;
    [Serializable]
    public class Screenshot
    {
        public Screenshot(byte[] screen)
        {
            this.Screen = screen;
        }

        public byte[] Screen { get; set; }

    }
}
