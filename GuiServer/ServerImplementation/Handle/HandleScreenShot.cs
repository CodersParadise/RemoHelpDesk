namespace GuiServer.ServerImplementation.Handle
{
    using GuiServer.Handle;
    using GuiServer.ServerImplementation.ViewModel;
    using GuiServer.ViewImplementation.Presenter;
    using GuiServer.ViewImplementation.Windows;
    using NetworkObjects;
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;

    public class HandleScreenshot : IHandlePacket
    {
        public void Handle(object receivedClass, ClientViewModel clientViewModel)
        {
            Screenshot screenShot = receivedClass as Screenshot;
            BitmapImage image = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(screenShot.Screen))
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
