using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MTGGatherer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DeckViewModel deckViewModel;
        private List<ScryfallSet> selectedSets;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Click_Parse(object sender, RoutedEventArgs e)
        {
            ParseButton.IsEnabled = false;
            string deckText = DeckText.Text;
            Deck deck = ParseDeckText(deckText);
            deckViewModel = new DeckViewModel();
            foreach (var card in deck.Cards)
            {
                ScryfallCard scryfallCard = null;
                if (card.CatID == 0 || OverrideCheckbox.IsChecked == true)
                {
                    foreach (ScryfallSet set in selectedSets)
                    {
                        scryfallCard = await GetScryfallCardByNameAsync(card.Name, set.Set);
                        if (scryfallCard.Set == set.Set) break;
                    }
                    if (scryfallCard == null || String.IsNullOrEmpty(scryfallCard.Name))
                    {
                        if (card.CatID == 0) scryfallCard = await GetScryfallCardByNameAsync(card.Name);
                        else scryfallCard = await GetScryfallCardByIDAsync(card.CatID);
                    }
                }
                else scryfallCard = await GetScryfallCardByIDAsync(card.CatID);

                if (scryfallCard == null) return;
                else if (scryfallCard.ImageUris == null && scryfallCard.CardFaces != null)
                {
                    scryfallCard.ImageUris = scryfallCard.CardFaces.FirstOrDefault().ImageUris;
                }

                for (int i = 0; i < card.Quantity; i++)
                    deckViewModel.AddCard(scryfallCard);
                DeckLoadTable.ItemsSource = deckViewModel.Cards;
            }
            DeckEditor deckEditor = new DeckEditor(deckViewModel);
            deckEditor.Show();
        }

        private Deck ParseDeckText(string deckText)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Deck));
                using (StringReader reader = new StringReader(deckText))
                {
                    return (Deck)serializer.Deserialize(reader);
                }
            }
            catch
            {
                return new Deck();
            }
        }

        private async Task<ScryfallCard> GetScryfallCardByIDAsync(int catId)
        {
            string url = $"https://api.scryfall.com/cards/mtgo/{catId}";
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
                HttpResponseMessage response = await client.GetAsync(url);
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

        private async Task<ScryfallCard> GetScryfallCardByNameAsync(string name, string set = "")
        {
            string url = $"https://api.scryfall.com/cards/named?fuzzy={name.Replace(" ", "+")}&set={set}";
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
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    return new ScryfallCard();
                    //MessageBox.Show(errorMessage);
                    //throw new HttpRequestException(errorMessage);
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ScryfallCard card = JsonConvert.DeserializeObject<ScryfallCard>(jsonResponse);
                return card;
            }
        }

        private void Click_Sets(object sender, RoutedEventArgs e)
        {
            SetSearch setSearch = new SetSearch();
            if (setSearch.ShowDialog() == true)
            {
                selectedSets = setSearch.selectedSets;
                SetsTextBox.Text = String.Join(", ", selectedSets.Select(x => x.Name));
            }
        }
    }
}