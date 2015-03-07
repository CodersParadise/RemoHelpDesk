namespace GuiServer.ViewImplementation.Presenter
{
    using GuiServer.ServerImplementation.ViewModel;
    using GuiServer.ViewImplementation.Windows;
    using NetworkObjects;
    using System;
    using System.IO;
    using System.Timers;
    using System.Windows.Media.Imaging;

    public class ScreenshotPresenter
    {
        private const long DEFAULT_QUALITY = 40L;
        private const int MIN_INTERVALL = 250;
        private const int MAX_INTERVALL = 5000;

        private ScreenshotWindow screenShotWindow;
        private ClientViewModel clientViewModel;
        private long quality;
        private int intervall;
        private Timer reloadTimer;
        private bool autoSave;

        public ScreenshotPresenter(ClientViewModel clientViewModel)
        {
            this.clientViewModel = clientViewModel;
            this.autoSave = false;
            this.clientViewModel.CanScreenshot = true;
            this.quality = ScreenshotPresenter.DEFAULT_QUALITY;
            this.intervall = ScreenshotPresenter.MIN_INTERVALL;
            this.reloadTimer = new Timer();
            this.reloadTimer.Elapsed += reloadTimer_Elapsed;
            this.reloadTimer.Interval = ScreenshotPresenter.MIN_INTERVALL;
        }

        public void SetScreenshotWindow(ScreenshotWindow screenShotWindow)
        {
            if (screenShotWindow != null && this.clientViewModel.CanScreenshot)
            {
                this.screenShotWindow = screenShotWindow;

                this.screenShotWindow.Closed += screenShotWindow_Closed;

                this.screenShotWindow.sliderQuality.Maximum = 100;
                this.screenShotWindow.sliderQuality.Minimum = 0;
                this.screenShotWindow.sliderQuality.Value = ScreenshotPresenter.DEFAULT_QUALITY;
                this.screenShotWindow.sliderQuality.ValueChanged += sliderQuality_ValueChanged;

                this.screenShotWindow.buttonReload.Click += buttonReload_Click;

                this.screenShotWindow.sliderIntervall.Minimum = ScreenshotPresenter.MIN_INTERVALL;
                this.screenShotWindow.sliderIntervall.Maximum = ScreenshotPresenter.MAX_INTERVALL;
                this.screenShotWindow.sliderIntervall.Value = ScreenshotPresenter.MAX_INTERVALL;
                this.screenShotWindow.sliderIntervall.ValueChanged += sliderIntervall_ValueChanged;

                this.screenShotWindow.checkboxAutoReload.Checked += checkboxAutoReload_Checked;
                this.screenShotWindow.checkboxAutoReload.Unchecked += checkboxAutoReload_Checked;

                this.screenShotWindow.checkboxAutoSave.IsChecked = this.autoSave;
                this.screenShotWindow.checkboxAutoSave.Checked += checkboxAutoSave_Checked;
                this.screenShotWindow.checkboxAutoSave.Unchecked += checkboxAutoSave_Checked;

            }
        }

        public void Show()
        {
            if (this.clientViewModel.CanScreenshot)
            {
                this.clientViewModel.CanScreenshot = false;
                this.screenShotWindow.Show();
                this.clientViewModel.SendObject(PacketId.SCREEN_SHOT, this.quality);
            }
        }

        public void UpdateScreenshot(BitmapImage bitmapImage)
        {
            Program.DispatchIfNecessary(() =>
            {
                this.screenShotWindow.imageView.Source = bitmapImage;
            });

            if (this.autoSave)
            {
                string path = this.GetScreenshotLocation();
                if (path != null)
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    using (Stream filestream = new FileStream(path, FileMode.Create))
                    {
                        encoder.Save(filestream);
                    }
                }
            }
        }

        private string GetScreenshotLocation()
        {
            string fullPath = null;
            string location = this.clientViewModel.UserPath + @"screenshots\";
            string fileName = "shot_" + DateTime.Now.ToString("yyyy-MM-dd H-m-s") + ".jpg";

            if ((location.Length > 0) && (!Directory.Exists(location)))
            {
                Directory.CreateDirectory(location);
            }


            if (Directory.Exists(location))
            {
                fullPath = location + fileName;
            }

            return fullPath;
        }

        private void checkboxAutoSave_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.autoSave = this.screenShotWindow.checkboxAutoSave.IsChecked.Value;
        }

        private void checkboxAutoReload_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.reloadTimer.Enabled = this.screenShotWindow.checkboxAutoReload.IsChecked.Value;
        }

        private void sliderIntervall_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            this.intervall = (int)e.NewValue;
            this.reloadTimer.Interval = this.intervall;
        }

        private void sliderQuality_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            this.quality = (long)e.NewValue;
        }

        private void buttonReload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.clientViewModel.SendObject(PacketId.SCREEN_SHOT, this.quality);
        }

        private void reloadTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.clientViewModel.SendObject(PacketId.SCREEN_SHOT, this.quality);
        }

        private void screenShotWindow_Closed(object sender, System.EventArgs e)
        {
            this.clientViewModel.CanScreenshot = true;
        }

    }
}
