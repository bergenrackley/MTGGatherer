using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace MTGGatherer
{
    public class Transform
    {
        public double posX { get; set; }
        public double posY { get; set; }
        public double posZ { get; set; }
        public double rotX { get; set; }
        public double rotY { get; set; }
        public double rotZ { get; set; }
        public double scaleX { get; set; }
        public double scaleY { get; set; }
        public double scaleZ { get; set; }

        public Transform(int mode)
        {
            switch (mode)
            {
                case 0:
                    posX = 0;
                    posY = 0;
                    posZ = 0;
                    rotX = 0;
                    rotY = 180;
                    rotZ = 180;
                    scaleX = 1; 
                    scaleY = 1; 
                    scaleZ = 1;
                    break;
                case 1:
                    posX = 0;
                    posY = 1;
                    posZ = 0;
                    rotX = 0;
                    rotY = 180;
                    rotZ = 180;
                    scaleX = 1;
                    scaleY = 1;
                    scaleZ = 1;
                    break;
                case 2:
                    posX = 2.2;
                    posY = 1;
                    posZ = 0;
                    rotX = 0;
                    rotY = 180;
                    rotZ = 0;
                    scaleX = 1;
                    scaleY = 1;
                    scaleZ = 1;
                    break;
            }
        }
    }

    public class TBCard
    {
        public int CardID { get; set; }
        public string Name { get; set; } = "Card";
        public string Nickname { get; set; }
        public Transform Transform { get; set; } = new Transform(0);
    }

    public class CustomCard
    {
        public string FaceURL { get; set; }
        public string BackURL { get; set; }
        public int NumHeight { get; set; } = 1;
        public int NumWidth { get; set; } = 1;
        public bool BackIsHidden { get; set; } = true;

        public CustomCard(string front, string back)
        {
            FaceURL = front;
            BackURL = back;
        }
    }

    public class DeckCustom
    {
        public string Name { get; set; } = "DeckCustom";
        public List<TBCard> ContainedObjects { get; set; } = new List<TBCard>();
        public List<int> DeckIDs { get; set; } = new List<int>();
        public Dictionary<string, CustomCard> CustomDeck { get; set; } = new Dictionary<string, CustomCard>();
        public Transform Transform { get; set; }
    }

    public class ExportDeck
    {
        public List<DeckCustom> ObjectStates { get; set; } = new List<DeckCustom>();
    }
}
