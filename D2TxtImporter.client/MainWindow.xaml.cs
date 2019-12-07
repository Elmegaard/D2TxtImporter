using System;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace D2TxtImporter.client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;

            _mainViewModel.ExcelPath = Properties.Settings.Default.ExcelPath;
            _mainViewModel.TablePath = Properties.Settings.Default.TablePath;
            _mainViewModel.OutputPath = Properties.Settings.Default.OutputPath;
            _mainViewModel.CubeRecipeUseDescription = Properties.Settings.Default.CubeRecipeUseDescription;
        }

        private void BrowseExcel(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();


            if (result == CommonFileDialogResult.Ok)
            {
                _mainViewModel.ExcelPath = dialog.FileName;
            }
        }

        private void BrowseTable(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                _mainViewModel.TablePath = dialog.FileName;
            }
        }

        private void BrowseOutput(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                _mainViewModel.OutputPath = dialog.FileName;
            }
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update settings
                Properties.Settings.Default.ExcelPath = _mainViewModel.ExcelPath;
                Properties.Settings.Default.TablePath = _mainViewModel.TablePath;
                Properties.Settings.Default.OutputPath = _mainViewModel.OutputPath;
                Properties.Settings.Default.CubeRecipeUseDescription = _mainViewModel.CubeRecipeUseDescription;
                Properties.Settings.Default.Save();

                // Import data
                _mainViewModel.Importer = new D2TxtImporter.lib.Importer(_mainViewModel.ExcelPath, _mainViewModel.TablePath, _mainViewModel.OutputPath);
                _mainViewModel.Importer.LoadData();
                _mainViewModel.Importer.ImportModel();

                _mainViewModel.OnPropertyChange(nameof(_mainViewModel.ExportEnabled));

                // Temporary Export, should be moved to its own button at some point
                _mainViewModel.Importer.Export();

                MessageBox.Show("Success!");
            }
            catch (Exception ex)
            {
                var errorMessage = "";
                do
                {
                    errorMessage += $"Message:\n{ex.Message}\n\n";
                    ex = ex.InnerException;
                }
                while (ex != null);
                var errorDialog = new ErrorDialog(errorMessage);
                errorDialog.Show();
            }
        }

        private void ExportData(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainViewModel.Importer.Export();
            }
            catch (Exception ex)
            {
                var errorDialog = new ErrorDialog(ex.Message + "\n\n" + ex.StackTrace);
                errorDialog.Show();
            }
        }
    }
}
