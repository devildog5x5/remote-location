using System.Windows;
using IPManagementInterface.Models;
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
            var addRuleWindow = new AddAlertRuleWindow(_monitoringService);
            if (addRuleWindow.ShowDialog() == true)
            {
                // Refresh the list
                DataContext = new { AlertRules = _monitoringService.GetAlertRules() };
            }
        }

        private void RemoveRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlertRulesDataGrid.SelectedItem is AlertRule selectedRule)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to remove the alert rule '{selectedRule.Name}'?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _monitoringService.RemoveAlertRule(selectedRule);
                    // Refresh the list
                    DataContext = new { AlertRules = _monitoringService.GetAlertRules() };
                }
            }
            else
            {
                MessageBox.Show("Please select an alert rule to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
