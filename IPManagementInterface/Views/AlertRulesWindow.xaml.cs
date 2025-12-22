using System.Windows;
using IPManagementInterface.Services;

namespace IPManagementInterface.Views
{
    public partial class AlertRulesWindow : Window
    {
        private readonly ScheduledMonitoringService _monitoringService;

        public AlertRulesWindow(ScheduledMonitoringService monitoringService)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            _monitoringService = monitoringService;

            DataContext = new { AlertRules = _monitoringService.GetAlertRules() };
        }

        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement add rule dialog
            MessageBox.Show("Add Rule functionality coming soon", "Alert Rules", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
