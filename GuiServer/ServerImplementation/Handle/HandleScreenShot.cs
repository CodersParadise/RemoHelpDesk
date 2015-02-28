namespace GuiServer.ServerImplementation.Handle
{
    using GuiServer.Handle;
    using GuiServer.ServerImplementation.ViewModel;
    using GuiServer.ViewImplementation.Windows;
    using NetworkObjects;
    using System;
    using System.IO;
    using System.Windows.Media.Imaging;

    public class HandleScreenShot : IHandlePacket
    {
        public void Handle(object receivedClass, ClientViewModel clientViewModel)
        {
            ScreenShot screenShot = receivedClass as ScreenShot;
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

            Program.DispatchIfNecessary(() =>
                {
                    ScreenShotWindow ssw = new ScreenShotWindow(image, "Client[" + clientViewModel.Id.ToString() + "]" + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"));
                    ssw.Show();
                });
        }
    }
}
