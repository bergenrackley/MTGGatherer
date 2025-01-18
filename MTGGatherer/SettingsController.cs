using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MTGGatherer
{
    public class SettingsController
    {
        private Dictionary<string, dynamic> defaults = new Dictionary<string, dynamic> { 
            { "CardBackUrl", "https://i.imgur.com/Hg8CwwU.jpeg" } 
        };
        public dynamic GetConfigurationValue(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? defaults[key];
        }
    }
}
