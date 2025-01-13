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
using Newtonsoft.Json.Serialization;

namespace MTGGatherer
{
    /// <summary>
    /// Interaction logic for SetSearch.xaml
    /// </summary>
    public partial class SetSearch : Window
    {
        private ScryfallSetSearch sets;
        private List<ScryfallSet> allSets;
        public List<ScryfallSet> selectedSets = new List<ScryfallSet>();
        public SetSearch()
        {
            InitializeComponent();
            LoadSets();
        }

        private async void LoadSets()
        {
            sets = await GetScryfallSetsAsync();
            //DataContext = sets;
            allSets = sets.Data;
            SetList.ItemsSource = sets.Data;
            SelectedSetsList.ItemsSource = selectedSets;
            SearchText.IsEnabled = true;
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
            SetList.ItemsSource = null;
            SetList.ItemsSource = sets.Data;
        }

        private void SetList_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                var checkBoxColumn = e.Column as DataGridCheckBoxColumn;
                if (checkBoxColumn != null)
                {
                    checkBoxColumn.IsReadOnly = false;
                    checkBoxColumn.Header = string.Empty;

                    Style checkBoxStyle = new Style(typeof(CheckBox)); 
                    checkBoxStyle.Setters.Add(new EventSetter(CheckBox.CheckedEvent, new RoutedEventHandler(CheckBox_Checked))); 
                    checkBoxStyle.Setters.Add(new EventSetter(CheckBox.UncheckedEvent, new RoutedEventHandler(CheckBox_Unchecked))); 
                    checkBoxColumn.ElementStyle = checkBoxStyle;
                }
            }
            else
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            DataGridRow dataGridRow = DataGridRow.GetRowContainingElement(checkBox);
            if (dataGridRow != null && !selectedSets.Contains(dataGridRow.Item as ScryfallSet))
            {
                ScryfallSet item = dataGridRow.Item as ScryfallSet;
                item.IsSelected = true;
                selectedSets.Add(item);
                SelectedSetsList.ItemsSource = null;
                SelectedSetsList.ItemsSource = selectedSets;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            DataGridRow dataGridRow = DataGridRow.GetRowContainingElement(checkBox);
            if (dataGridRow != null && selectedSets.Contains(dataGridRow.Item as ScryfallSet))
            {
                ScryfallSet item = dataGridRow.Item as ScryfallSet;
                item.IsSelected = false;
                selectedSets.Remove(item);
                SelectedSetsList.ItemsSource = null;
                SelectedSetsList.ItemsSource = selectedSets;
            }
        }

        private void SelectedSetsList_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "IsSelected") e.Cancel = true;
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            DataGridRow dataGridRow = DataGridRow.GetRowContainingElement(button);
            var item = dataGridRow.Item as ScryfallSet;
            int index = selectedSets.IndexOf(item);
            if (index != 0) index -= 1;
            selectedSets.Remove(item);
            selectedSets.Insert(index, item);

            SelectedSetsList.ItemsSource = null;
            SelectedSetsList.ItemsSource = selectedSets;
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            DataGridRow dataGridRow = DataGridRow.GetRowContainingElement(button);
            var item = dataGridRow.Item as ScryfallSet;
            int index = selectedSets.IndexOf(item);
            if (index < selectedSets.Count - 1) index += 1;
            selectedSets.Remove(item);
            selectedSets.Insert(index, item);

            SelectedSetsList.ItemsSource = null;
            SelectedSetsList.ItemsSource = selectedSets;
        }

        private void Click_Save(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
