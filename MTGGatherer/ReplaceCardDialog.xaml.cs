using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
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
        public ScryfallCard replacementCard;

        public ReplaceCardDialog()
        {
            InitializeComponent();
        }

        private async void Click_Search(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchText.Text))
            {
                SearchButton.IsEnabled = false;
                ScryfallCard searchCard = await GetScryfallCardByNameAsync(SearchText.Text);
                if (searchCard != null && !string.IsNullOrEmpty(searchCard.Name))
                {
                    if (MessageBox.Show($"Is this your card: {searchCard.Name}",
                        "Is this your card?",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        replacementCard = searchCard;
                        this.DialogResult = true;
                        this.Close();
                    }
                } else
                {
                    MessageBox.Show("Card not found, check for typos or be more specific.", "Error");
                }
                SearchButton.IsEnabled = true;
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
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    return new ScryfallCard();
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ScryfallCard card = JsonConvert.DeserializeObject<ScryfallCard>(jsonResponse);

                return card;
            }
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
