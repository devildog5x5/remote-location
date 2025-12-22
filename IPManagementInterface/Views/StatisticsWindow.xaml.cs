using System.Windows;
using IPManagementInterface.Services;
using IPManagementInterface.Models;

namespace IPManagementInterface.Views
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(ReportingService reportingService, System.Collections.Generic.IEnumerable<IoTDevice> devices)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            var statistics = reportingService.GetStatistics(devices);
            var uptimeReport = reportingService.GetUptimeReport(devices);

            DataContext = new
            {
                Statistics = statistics,
                UptimeReport = uptimeReport
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
