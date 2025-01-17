using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
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
using System.Diagnostics;

namespace MTGGatherer
{
    /// <summary>
    /// Interaction logic for PrintsView.xaml
    /// </summary>
    public partial class PrintsView : Window
    {
        private List<ScryfallCard> prints;
        public string name;
        public ScryfallCard selectedPrint;
        
        public PrintsView(string cardName)
        {
            InitializeComponent();
            LoadCards(cardName);
        }

        public async void LoadCards(string cardName)
        {
            ScryfallSetSearch sets = await GetScryfallSetsAsync();
            SetsComboBox.ItemsSource = sets.Data;
            name = cardName;
            ScryfallSearch search = await GetAlternatePrintsAsync();
            prints = search.Data;
            CardItemsControl.ItemsSource = prints;
            SetsComboBox.IsEnabled = true;
        }

        private async Task<ScryfallSetSearch> GetScryfallSetsAsync()
        {
            string url = "https://api.scryfall.com/sets";
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
                ScryfallSetSearch search = JsonConvert.DeserializeObject<ScryfallSetSearch>(jsonResponse);
                return search;
            }
        }

        private async Task<ScryfallSearch> GetAlternatePrintsAsync(string set = null)
        {
            if (set != null) set = " e:" + set;
            string url = $"https://api.scryfall.com/cards/search?unique=prints&q={WebUtility.UrlEncode(name)}{set}";
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
                    return new ScryfallSearch();
                    throw new HttpRequestException(errorMessage);
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ScryfallSearch prints = JsonConvert.DeserializeObject<ScryfallSearch>(jsonResponse);
                prints.Data = prints.Data.Where(e => e.Name == name).ToList();
                return prints;
            }
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

        private async void RefinePrints(object sender, SelectionChangedEventArgs e)
        {
            ScryfallSet selectedSet = SetsComboBox.SelectedItem as ScryfallSet;
            if (selectedSet != null && selectedSet.Set != null)
            {
                CardItemsControl.ItemsSource = null;
                ScryfallSearch setSearch = await GetAlternatePrintsAsync(selectedSet.Set);
                prints = setSearch.Data;
                CardItemsControl.ItemsSource = prints;
            }
        }
    }
}
