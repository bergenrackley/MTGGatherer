using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace MTGGatherer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Click_Parse(object sender, RoutedEventArgs e)
        {
            string deckText = DeckText.Text;
            Deck deck = ParseDeckText(deckText);
            foreach (var card in deck.Cards)
            {
                if (card.CatID == 0)
                {
                    ScryfallCard scryfallCard = await GetScryfallCardByNameAsync(card.Name);
                    Trace.WriteLine($"Card Name: {scryfallCard.Name}, Set: {scryfallCard.SetName}");
                } else
                {
                    ScryfallCard scryfallCard = await GetScryfallCardByIDAsync(card.CatID); 
                    Trace.WriteLine($"Card Name: {scryfallCard.Name}, Set: {scryfallCard.SetName}");
                }
            }
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
            } catch
            {
                return new Deck();
            }
        }

        private async Task<ScryfallCard> GetScryfallCardByIDAsync(int catId) { 
            string url = $"https://api.scryfall.com/cards/mtgo/{catId}"; 
            using (HttpClient client = new HttpClient()) { 
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
                String test = await response.Content.ReadAsStringAsync();
                Trace.WriteLine(test);
                if (!response.IsSuccessStatusCode) { 
                    string errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}"; 
                    MessageBox.Show(errorMessage); 
                    throw new HttpRequestException(errorMessage); 
                } string jsonResponse = await response.Content.ReadAsStringAsync(); 
                ScryfallCard card = JsonConvert.DeserializeObject<ScryfallCard>(jsonResponse); 
                return card; 
            } 
        }

        private async Task<ScryfallCard> GetScryfallCardByNameAsync(string name)
        {
            string url = $"https://api.scryfall.com/cards/named?fuzzy={name.Replace(" ", "+")}";
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
                String test = await response.Content.ReadAsStringAsync();
                Trace.WriteLine(test);
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