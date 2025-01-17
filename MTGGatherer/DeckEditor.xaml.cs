using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Net.Http.Headers;
using System.Net.Http;

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

        private async void Click_Export(object sender, RoutedEventArgs e)
        {
            ExportDeck exportDeck = new ExportDeck();

            DeckCustom mainDeck = new DeckCustom();
            DeckCustom extrasDeck = new DeckCustom();
            List<String> mainArtUrls = new List<String>();
            List<String> extraArtUrls = new List<String>();
            foreach (ScryfallCard card in deckViewModel.Cards)
            {
                if (!mainArtUrls.Contains(card.GetFace())) mainArtUrls.Add(card.GetFace());
                TBCard tbCard = new TBCard
                {
                    CardID = (mainArtUrls.IndexOf(card.GetFace()) + 1) * 100, //Should work correctly but will want to add a control later to pick which resolution the program exports with
                    Nickname = card.FlavorName ?? card.Name
                };
                mainDeck.ContainedObjects.Add(tbCard);

                if (card.AllParts != null && card.AllParts.Any())
                {
                    foreach (RelatedCard relatedCard in card.AllParts.Where(relatedCard => relatedCard.Object == "related_card" && !deckViewModel.Cards.Where(e => relatedCard.Name == (e.FlavorName ?? e.Name)).Any() && !extrasDeck.ContainedObjects.Where(e => relatedCard.Name == e.Nickname).Any()).ToList())
                    {
                        ScryfallCard newCard = await GetScryfallCardByUri(relatedCard.Uri);
                        MessageBox.Show(newCard.Name, "Error");
                        extraArtUrls.Add(newCard.GetFace());
                        TBCard extraCard = new TBCard
                        {
                            CardID = extraArtUrls.Count() * 100,
                            Nickname = newCard.FlavorName ?? newCard.Name
                        };
                        extrasDeck.ContainedObjects.Add(extraCard);
                    };
                }
            }
            mainDeck.DeckIDs = mainDeck.ContainedObjects.Select(e => e.CardID).Distinct().ToList();
            mainArtUrls.Select((url, index) => new { Index = index + 1, CustomCard = new CustomCard(url, "https://i.imgur.com/Hg8CwwU.jpeg") }).ToList().ForEach(item => mainDeck.CustomDeck.Add(item.Index.ToString(), item.CustomCard));
            mainDeck.Transform = new Transform(1);
            exportDeck.ObjectStates.Add(mainDeck);

            extrasDeck.DeckIDs = extrasDeck.ContainedObjects.Select(e => e.CardID).Distinct().ToList();
            extraArtUrls.Select((url, index) => new { Index = index + 1, CustomCard = new CustomCard(url, "https://i.imgur.com/Hg8CwwU.jpeg") }).ToList().ForEach(item => extrasDeck.CustomDeck.Add(item.Index.ToString(), item.CustomCard));


            SaveFileDialog saveFileDialog = new SaveFileDialog(); saveFileDialog.Filter = "JSON files (*.json)|*.json"; if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName; 
                System.IO.File.WriteAllText(filePath, JsonConvert.SerializeObject(exportDeck, Formatting.Indented));
            }
        }

        private async Task<ScryfallCard> GetScryfallCardByUri(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/avif"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/apng"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/signed-exchange", 0.7));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                HttpResponseMessage response = await client.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    MessageBox.Show(errorMessage);
                    throw new HttpRequestException(errorMessage);
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ScryfallCard card = JsonConvert.DeserializeObject<ScryfallCard>(jsonResponse);
                return card;
            }
        }
    }

}
