using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MTGGatherer
{
    public partial class DeckEditor : Window
    {
        private DeckViewModel deckViewModel;
        public DeckEditor(DeckViewModel DVM)
        {
            InitializeComponent();
            deckViewModel = DVM;
            CardItemsListBox.ItemsSource = deckViewModel.Cards;
        }

        private void CardItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardItemsListBox.SelectedItem is ScryfallCard card)
            {
                CardItemsListBox.IsEnabled = false;
                int index = CardItemsListBox.SelectedIndex;
                Trace.WriteLine(CardItemsListBox.SelectedIndex);
                CardEditor cardEditor = new CardEditor(card);
                if (cardEditor.ShowDialog() == true)
                {
                    ScryfallCard selectedCard = cardEditor.card;
                    deckViewModel.Cards[index] = selectedCard;
                }
                CardItemsListBox.IsEnabled = true;
                CardItemsListBox.SelectedItem = null;
            }
        }

    }
}
