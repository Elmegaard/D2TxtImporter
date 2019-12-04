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
using System.Windows.Shapes;

namespace D2TxtImporter.client
{
    /// <summary>
    /// Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public ErrorDialog(string errorMessage)
        {
            InitializeComponent();
            ResponseTextBox.Text = errorMessage;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
