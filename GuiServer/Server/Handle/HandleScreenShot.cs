namespace GuiServer.Server.Handle
{
    using GuiServer.Handle;
    using GuiServer.View.ViewModel;
    using GuiServer.View.Presenter;
    using GuiServer.View.Windows;
    using NetworkObjects;
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;

    public class HandleScreenshot : IHandlePacket
    {
        public void Handle(object receivedClass, ClientViewModel clientViewModel, Server server)
        {
            byte[] screenShot = receivedClass as byte[];
            BitmapImage image = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(screenShot))
                {
                    image = new BitmapImage();

                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                   
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

         
            clientViewModel.UpdateScreenshotOutput(image);


        }
    }
}
