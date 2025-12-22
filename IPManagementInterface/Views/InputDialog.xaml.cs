using System.Windows;

namespace IPManagementInterface.Views
{
    public partial class InputDialog : Window
    {
        public string ResponseText { get; private set; } = string.Empty;

        public InputDialog(string title, string prompt)
        {
            InitializeComponent();
            Title = title;
            PromptText.Text = prompt;
            InputTextBox.Focus();
            InputTextBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    OkButton_Click(s, e);
                }
            };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = InputTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
