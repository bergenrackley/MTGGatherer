using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGGatherer
{
    public class ScryfallCard
    {
        [JsonProperty(PropertyName = "mtgo_id")]

        public int MtgoId { get; set; }
        public string Name { get; set; }
        [JsonProperty(PropertyName = "image_uris")]

        public ImageUris ImageUris { get; set; }
        [JsonProperty(PropertyName = "set_id")]

        public string SetId { get; set; }
        public string Set { get; set; }
        [JsonProperty(PropertyName = "set_name")]

        public string SetName { get; set; }
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

}
