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

namespace MTGGatherer
{
    /// <summary>
    /// Interaction logic for ReplaceCardDialog.xaml
    /// </summary>
    public partial class ReplaceCardDialog : Window
    {
        public ReplaceCardDialog()
        {
            InitializeComponent();
        }

        private void Click_Search(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
