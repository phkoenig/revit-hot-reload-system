using System;
using System.Windows;

namespace GeoJsonImporter.Work.UI
{
    public partial class GeoRefDialog : Window
    {
        public string EnteredAddress { get; private set; } = "";

        public GeoRefDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            EnteredAddress = AddressTextBox.Text?.Trim() ?? "";
            
            if (string.IsNullOrEmpty(EnteredAddress))
            {
                System.Windows.MessageBox.Show("Bitte geben Sie eine Adresse ein.", "Fehler", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
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
