using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MTGGatherer
{
    public class DeckViewModel
    {
        public ObservableCollection<ScryfallCard> Cards { get; set; } = new ObservableCollection<ScryfallCard>();

        public void AddCard(ScryfallCard card)
        {
            Cards.Add(card);
        }

        public DeckViewModel() { }

        public DeckViewModel(ScryfallSearch scryfallSearch)
        {
            scryfallSearch.Data.ForEach(Cards.Add);
        }
    }
}
