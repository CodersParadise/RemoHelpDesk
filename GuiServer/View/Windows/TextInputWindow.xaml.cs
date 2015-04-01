namespace GuiServer.View.Windows
{
    using System.Windows;
    using System.Windows.Input;

    public partial class TextInputWindow : Window
    {
        public static string ShowWindow(string title, string text)
        {
            return TextInputWindow.ShowWindow(null, title, text);
        }

        public static string ShowWindow(Window owner, string title, string text)
        {
            TextInputWindow textInputWindow = new TextInputWindow(title, text);
            textInputWindow.Owner = owner;
            bool? dialogResult = textInputWindow.ShowDialog();
            if (dialogResult == true)
            {
                return textInputWindow.UserInput;
            }
            else
            {
                return null;
            }
        }

        public TextInputWindow(string title, string text)
        {
            InitializeComponent();
            if (this.Owner != null)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            this.UserInput = string.Empty;
            this.Title = title;
            this.textblockInfo.Text = text;
            this.buttonAccept.Click += buttonAccept_Click;
            this.buttonCancel.Click += buttonCancel_Click;
            this.textboxInput.KeyDown += textboxInput_KeyDown;
            this.textboxInput.Focus();
        }

        public string UserInput { get; private set; }

        private void AcceptInput()
        {
            this.UserInput = textboxInput.Text;
            this.DialogResult = true;
        }

        private void textboxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.AcceptInput();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            this.AcceptInput();
        }

    }
}
