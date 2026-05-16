using Microsoft.Win32;
using System.Windows.Controls;

namespace EasySave.WPF.Views
{
    public partial class JobListView : UserControl
    {
        public JobListView()
        {
            InitializeComponent();
        }

        // Add form — browse handlers
        private void BrowseAddSource_Click(object sender, System.Windows.RoutedEventArgs e)
            => BrowseFolder(TxtAddSource);

        private void BrowseAddTarget_Click(object sender, System.Windows.RoutedEventArgs e)
            => BrowseFolder(TxtAddTarget);

        // Edit card — browse handlers
        private void BrowseEditSource_Click(object sender, System.Windows.RoutedEventArgs e)
            => BrowseFolder(TxtEditSource);

        private void BrowseEditTarget_Click(object sender, System.Windows.RoutedEventArgs e)
            => BrowseFolder(TxtEditTarget);

        /// <summary>
        /// Opens a folder picker dialog and writes the selected path into <paramref name="textBox"/>.
        /// The two-way binding propagates the value to the ViewModel automatically.
        /// </summary>
        private static void BrowseFolder(TextBox textBox)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Sélectionner un dossier",
                InitialDirectory = string.IsNullOrWhiteSpace(textBox.Text)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    : textBox.Text
            };

            if (dialog.ShowDialog() == true)
                textBox.Text = dialog.FolderName;
        }
    }
}
