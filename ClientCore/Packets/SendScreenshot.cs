﻿namespace ClientCore.Packets
{
    using NetworkObjects;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows;
    using Arrowgene.Services.Network.ManagedConnection.Client;

    public class SendScreenshot : ISendPacket
    {
        private long quality;

        public SendScreenshot(long quality)
        {
            this.quality = quality;
        }

        public void Send(ClientSocket serverSocket)
        {
            byte[] screenShot = null;

            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            Encoder myEncoder = Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, this.quality);
            myEncoderParameters.Param[0] = myEncoderParameter;

            using (Bitmap bmp = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, jgpEncoder, myEncoderParameters);
                    screenShot = ms.ToArray();
                }
            }

   

            serverSocket.SendObject(PacketId.SCREEN_SHOT, screenShot);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
