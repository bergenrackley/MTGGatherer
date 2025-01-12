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
    /// Interaction logic for PrintsView.xaml
    /// </summary>
    public partial class PrintsView : Window
    {
        private DeckViewModel deckViewModel;
        public ScryfallCard selectedPrint;
        public PrintsView(DeckViewModel DVM)
        {
            InitializeComponent();
            deckViewModel = DVM;
            foreach (ScryfallCard card in deckViewModel.Cards)
            {
                if (card.ImageUris == null && card.CardFaces != null)
                {
                    card.ImageUris = card.CardFaces.FirstOrDefault().ImageUris;
                }
            }
            CardItemsControl.ItemsSource = deckViewModel.Cards;
        }

        private void Card_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is ScryfallCard card)
            {
                //MessageBox.Show($"Selected print {card.Name}");
                selectedPrint = card;
                DialogResult = true;
                Close();
            }
        }
    }
}
