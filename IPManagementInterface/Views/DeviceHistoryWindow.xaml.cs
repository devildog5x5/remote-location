using System.Windows;
using IPManagementInterface.Services;

namespace IPManagementInterface.Views
{
    public partial class DeviceHistoryWindow : Window
    {
        public DeviceHistoryWindow(DeviceHistoryService historyService, string? deviceIp = null)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            if (!string.IsNullOrEmpty(deviceIp))
            {
                DataContext = new { History = historyService.GetHistoryForDevice(deviceIp) };
            }
            else
            {
                DataContext = new { History = historyService.GetRecentHistory(100) };
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
