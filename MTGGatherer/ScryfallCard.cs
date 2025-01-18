using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace MTGGatherer
{
    public class ScryfallSearch
    {
        [JsonProperty(PropertyName = "total_cards")]
        public int TotalCards { get; set; }
        [JsonProperty(PropertyName = "has_more")]
        public bool HasMore { get; set; }
        [JsonProperty(PropertyName = "data")]
        public List<ScryfallCard> Data { get; set; }
    }

    public class ScryfallCard
    {
        [JsonProperty(PropertyName = "mtgo_id")]
        public int MtgoId { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "flavor_name")]
        public string FlavorName { get; set; }
        [JsonProperty(PropertyName = "image_uris")]
        public ImageUris ImageUris { get; set; }
        [JsonProperty(PropertyName = "set_id")]
        public string SetId { get; set; }
        [JsonProperty(PropertyName = "card_faces")]
        public List<CardFace> CardFaces { get; set; }
        [JsonProperty(PropertyName = "set")]
        public string Set { get; set; }
        [JsonProperty(PropertyName = "set_name")]
        public string SetName { get; set; }
        [JsonProperty(PropertyName = "collector_number")]
        public string CollectorNumber { get; set; }
        [JsonProperty(PropertyName = "all_parts")]
        public List<RelatedCard> AllParts { get; set; }

        public string GetFace()
        {
            if (ImageUris == null && CardFaces != null)
            {
                return CardFaces.FirstOrDefault().ImageUris.Normal;
            }
            else
            {
                return ImageUris.Normal;
            }
        }

        public List<String> GetFaces()
        {
            SettingsController settingsController = new SettingsController();
            return CardFaces?.Select(card => card.ImageUris.Normal).ToList() ?? new List<String> { ImageUris.Normal, settingsController.GetConfigurationValue("CardBackUrl") };
        }
    }

    public class CardFace
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "image_uris")]
        public ImageUris ImageUris { get; set; }
    }

    public class ImageUris
    {
        public string Small { get; set; }
        public string Normal { get; set; }
        public string Large { get; set; }
        public string Png { get; set; }
        [JsonProperty(PropertyName = "art_crop")]
        public string ArtCrop { get; set; }
        [JsonProperty(PropertyName = "border_crop")]
        public string BorderCrop { get; set; }
    }

    public class RelatedCard
    {
        [JsonProperty(PropertyName = "object")]
        public string Object { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }
    }

    public class GetFaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ScryfallCard card)
            {
                return card.GetFace();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}