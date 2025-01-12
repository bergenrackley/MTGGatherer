using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGGatherer
{
    public class ScryfallSetSearch
    {
        [JsonProperty(PropertyName = "has_more")]
        public bool HasMore { get; set; }
        [JsonProperty(PropertyName = "data")]
        public List<ScryfallSet> Data { get; set; }
    }

    public class ScryfallSet
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "code")]
        public string Set { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "icon_svg_uri")]
        public string IconUri { get; set; }
    }
}