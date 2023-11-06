using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CognexDataViewer.Controls
{
    /// <summary>
    /// Interaction logic for TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
            PopulateComboBoxes();
        }

        private void PopulateComboBoxes()
        {
            // Populate Hour ComboBox
            for (int i = 1; i <= 12; i++)
            {
                HourComboBox.Items.Add(i.ToString()); // "D2" for zero-padding
            }

            // Populate Minute ComboBox
            for (int i = 0; i < 60; i++)
            {
                MinuteComboBox.Items.Add(i.ToString("D2"));
            }

            // Populate Period (AM/PM) ComboBox
            PeriodComboBox.Items.Add("AM");
            PeriodComboBox.Items.Add("PM");
        }

        public static readonly DependencyProperty SelectedTimeProperty =
        DependencyProperty.Register(
            "SelectedTime",
            typeof(string), // You can adjust the type based on your needs
            typeof(TimePicker),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SelectedTime
        {
            get { return (string)GetValue(SelectedTimeProperty); }
            set { SetValue(SelectedTimeProperty, value); }
        }

        // Event handlers for ComboBox selection changes
        private void HourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedTime();
        }

        private void MinuteComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedTime();
        }

        private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedTime();
        }

        // Method to update the SelectedTime property based on ComboBox selections
        private void UpdateSelectedTime()
        {
            // Combine the selected values from the ComboBoxes into a single string
            string selectedHour = HourComboBox.SelectedItem?.ToString();
            string selectedMinute = MinuteComboBox.SelectedItem?.ToString();
            string selectedPeriod = PeriodComboBox.SelectedItem?.ToString();

            // Construct the selected time string (adjust format as needed)
            SelectedTime = $"{selectedHour}:{selectedMinute} {selectedPeriod}";
        }
    }
}

