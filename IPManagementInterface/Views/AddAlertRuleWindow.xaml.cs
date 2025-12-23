using System;
using System.Windows;
using IPManagementInterface.Models;
using IPManagementInterface.Services;

namespace IPManagementInterface.Views
{
    public partial class AddAlertRuleWindow : Window
    {
        private readonly ScheduledMonitoringService _monitoringService;
        public AlertRule? CreatedRule { get; private set; }

        public AddAlertRuleWindow(ScheduledMonitoringService monitoringService)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            _monitoringService = monitoringService;

            // Set defaults
            ConditionComboBox.SelectedIndex = 0;
            ActionComboBox.SelectedIndex = 0;
        }

        private void AddRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RuleNameTextBox.Text))
            {
                MessageBox.Show("Please enter a rule name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ConditionComboBox.SelectedItem == null || ActionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select both condition and action.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var conditionStr = ((System.Windows.Controls.ComboBoxItem)ConditionComboBox.SelectedItem).Tag?.ToString();
            var actionStr = ((System.Windows.Controls.ComboBoxItem)ActionComboBox.SelectedItem).Tag?.ToString();

            if (!Enum.TryParse<AlertCondition>(conditionStr, out var condition) ||
                !Enum.TryParse<AlertAction>(actionStr, out var action))
            {
                MessageBox.Show("Invalid condition or action selected.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var rule = new AlertRule
            {
                Name = RuleNameTextBox.Text,
                Condition = condition,
                Action = action,
                IsEnabled = EnabledCheckBox.IsChecked ?? true
            };

            if (condition == AlertCondition.DeviceOfflineForDuration && int.TryParse(DurationTextBox.Text, out var duration))
            {
                rule.OfflineDurationMinutes = duration;
            }

            _monitoringService.AddAlertRule(rule);
            CreatedRule = rule;
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
