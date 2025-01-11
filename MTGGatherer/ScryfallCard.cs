using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGGatherer
{
    public class ScryfallCard
    {
        public int MtgoId { get; set; }
        public string Name { get; set; }
        public ImageUris ImageUris { get; set; }
        public string SetId { get; set; }
        public string Set { get; set; }
        public string SetName { get; set; }
    }

    public class ImageUris
    {
        public string Small { get; set; }
        public string Normal { get; set; }
        public string Large { get; set; }
        public string Png { get; set; }
        public string ArtCrop { get; set; }
        public string BorderCrop { get; set; }
    }

}
