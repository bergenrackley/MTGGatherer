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
using System.IO;
using System.Diagnostics;

namespace MTGGatherer
{
    /// <summary>
    /// Interaction logic for SetSearch.xaml
    /// </summary>
    public partial class SetSearch : Window
    {
        private ScryfallSetSearch sets;
        private List<ScryfallSet> allSets;
        public SetSearch()
        {
            InitializeComponent();
            LoadSets();
        }

        private async void LoadSets()
        {
            sets = await GetScryfallSetsAsync();
            DataContext = sets;
            allSets = sets.Data;
            SetList.ItemsSource = sets.Data;
            SearchText.Focus();
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

        private void RefineSets(object sender, TextChangedEventArgs e)
        {
            sets.Data = allSets.Where(set => set.Name.Contains(SearchText.Text, StringComparison.OrdinalIgnoreCase)).ToList();
            SetList.ItemsSource = sets.Data;
        }
    }
}
