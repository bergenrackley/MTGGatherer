﻿using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
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
        private List<ScryfallSet> selectedSets = new List<ScryfallSet>();
        SettingsController settingsController = new SettingsController();

        public MainWindow()
        {
            InitializeComponent();

            string jsonString = settingsController.GetConfigurationValue("SetsList");
            if (!string.IsNullOrWhiteSpace(jsonString) && IsValidJson(jsonString))
            {
                try
                {
                    selectedSets = JsonConvert.DeserializeObject<List<ScryfallSet>>(jsonString);
                }
                catch
                {
                    selectedSets = new List<ScryfallSet>();

                }
            }

            LoadSetsList(selectedSets);
        }

        private bool IsValidJson(string jsonString) { 
            try { 
                JToken.Parse(jsonString); 
                return true; 
            } catch (JsonReaderException) { 
                return false;
            } 
        }

        public void LoadSetsList(List<ScryfallSet> sets)
        {
            selectedSets = sets;
            settingsController.SaveSettings(JsonConvert.SerializeObject(selectedSets, Formatting.Indented), "SetsList");
            SetsTextBox.Text = String.Join(", ", selectedSets.Select(x => x.Name));
        }

        private async void Click_Parse(object sender, RoutedEventArgs e)
        {
            ParseButton.IsEnabled = false;
            FileButton.IsEnabled = false;
            DeckText.IsEnabled = false;
            SetsButton.IsEnabled = false;
            OverrideCheckbox.IsEnabled = false;

            string deckText = DeckText.Text;
            Deck deck = ParseDeckText(deckText);
            if (deck == null || deck.Cards == null || deck.Cards.Count == 0)
            {
                MessageBox.Show("Error procesing file, check xaml or reexport", "Error");
            }
            else
            {
                deckViewModel = new DeckViewModel();
                foreach (var card in deck.Cards)
                {
                    ScryfallCard scryfallCard = null;
                    if (card.CatID == 0 || OverrideCheckbox.IsChecked == true)
                    {
                        //foreach (ScryfallSet set in selectedSets)
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

                    scryfallCard.Sideboard = card.Sideboard;
                    for (int i = 0; i < card.Quantity; i++)
                        deckViewModel.AddCard(scryfallCard);
                    DeckLoadTable.ItemsSource = deckViewModel.Cards;
                }
                DeckEditor deckEditor = new DeckEditor(deckViewModel);
                if (deckEditor.ShowDialog() == true)
                {

                }
            }

            ParseButton.IsEnabled = true;
            FileButton.IsEnabled = true;
            DeckText.IsEnabled = true;
            SetsButton.IsEnabled = true;
            OverrideCheckbox.IsEnabled = true;
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
                }
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ScryfallCard card = JsonConvert.DeserializeObject<ScryfallCard>(jsonResponse);
                return card;
            }
        }

        private void Click_Sets(object sender, RoutedEventArgs e)
        {
            SetSearch setSearch = new SetSearch();
            if (setSearch.ShowDialog() == true) LoadSetsList(setSearch.selectedSets);
        }

        private void Click_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName; 
                string fileContent = File.ReadAllText(filePath); 
                DeckText.Text = fileContent;
            }
        }
    }
}