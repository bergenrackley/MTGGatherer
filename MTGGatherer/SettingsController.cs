using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MTGGatherer
{
    public class SettingsController
    {
        private Dictionary<string, dynamic> defaults = new Dictionary<string, dynamic> { 
            { "CardBackUrl", "https://i.imgur.com/Hg8CwwU.jpeg" },
            { "SetsList", "" }
        };

        public dynamic GetConfigurationValue(string key)
        {
            return string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) ? defaults[key] : ConfigurationManager.AppSettings[key];
        }

        public void SaveSettings(string value, string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
