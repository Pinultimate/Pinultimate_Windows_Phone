using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Pinultimate_Windows_Phone
{
    public class AppSettings
    {
        // Our settings
        private IsolatedStorageSettings settings;

        // The key names of our settings
        public const string TrackerSettingKeyName = "TrackerSetting";
        public const string ThemeColorSettingKeyName = "ThemeColorSetting";

        // The default value of our settings
        private const bool TrackerSettingDefault = true;
        private int ListBoxSettingDefault = 2; // default is Blue

        private List<Color> themeColors = new List<Color> { Colors.Red, Colors.Orange, Colors.Blue };

        public Color ThemeColor
        {
            get
            {
                return themeColors[ThemeColorPosition];
            }
        }


        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettings()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        private void Save()
        {
            settings.Save();
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public bool Contains(String key)
        {
            return settings.Contains(key);
        }

        /// <summary>
        /// Property to get and set a CheckBox Setting Key.
        /// </summary>
        public bool TrackingSetting
        {
            get
            {
                return GetValueOrDefault<bool>(TrackerSettingKeyName, TrackerSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(TrackerSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Property to get and set a ListBox Setting Key.
        /// </summary>
        public int ThemeColorPosition
        {
            get
            {
                return GetValueOrDefault<int>(ThemeColorSettingKeyName, ListBoxSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(ThemeColorSettingKeyName, value))
                {
                    Save();
                }
            }
        }
    }
}
