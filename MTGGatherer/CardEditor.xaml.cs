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
            ScryfallSearch search = await GetAlternatePrintsAsync(card.Name);
            DeckViewModel deckViewModel = new DeckViewModel(search);
            PrintsView printsView = new PrintsView(deckViewModel);
            if (printsView.ShowDialog() == true)
            {
                ScryfallCard selectedCard = printsView.selectedPrint; 
                card = selectedCard;
                DataContext = card;
            }
            AltPrints.IsEnabled = true;
            Save.IsEnabled = true;
        }

        private async Task<ScryfallSearch> GetAlternatePrintsAsync(string name)
        {
            string url = $"https://api.scryfall.com/cards/search?unique=prints&q={WebUtility.UrlEncode(name)}";
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
                ScryfallSearch prints = JsonConvert.DeserializeObject<ScryfallSearch>(jsonResponse);
                return prints;
            }
        }

        private void Click_Save(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
