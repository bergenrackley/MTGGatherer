using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Printing;
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
using System.Net;

namespace MTGGatherer
{
    public partial class CardEditor : Window
    {
        public ScryfallCard card;
        public CardEditor(ScryfallCard c)
        {
            card = c;
            DataContext = card;
            InitializeComponent();
        }

        private async void Click_AltPrints(object sender, RoutedEventArgs e)
        {
            AltPrints.IsEnabled = false;
            Save.IsEnabled = false;
            PrintsView printsView = new PrintsView(card.Name);
            if (printsView.ShowDialog() == true)
            {
                ScryfallCard selectedCard = printsView.selectedPrint; 
                card = selectedCard;
                DataContext = card;
            }
            AltPrints.IsEnabled = true;
            Save.IsEnabled = true;
        }

        private void Click_Save(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
