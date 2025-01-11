using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MTGGatherer
{
    public class Deck
    {
        public int NetDeckID { get; set; }
        public int PreconstructedDeckID { get; set; }
        [XmlElement("Cards")]
        public List<Card> Cards { get; set; }
    }

    public class Card
    {
        [XmlAttribute("CatID")]
        public int CatID { get; set; }
        [XmlAttribute("Quantity")]
        public int Quantity { get; set; }
        [XmlAttribute("Sideboard")]
        public bool Sideboard { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }
}