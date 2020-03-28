using System;
using System.Collections.Generic;
using System.IO;
using HREngine.Bots;

namespace Silverfish.Routines.DefaultRoutine.Silverfish.Helpers
{
    public class DiscoverHelper
    {
        private static string _discoverFilePath = string.Empty;
        private static Dictionary<string, float> _discoverDictionary;

        internal static void Init()
        {
            _discoverFilePath = Path.Combine(Settings.Instance.path, "discover.ini");
            _discoverDictionary = new Dictionary<string, float>();
            var lines = File.ReadAllLines(_discoverFilePath);
            string name = string.Empty;
            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    name = line.TrimStart('[');
                    name = name.TrimEnd(']');
                }
                else
                {
                    var array = line.Split('=');
                    var valueString = array[1];
                    valueString = valueString.Trim(' ');
                    var value = Convert.ToSingle(valueString);
                    _discoverDictionary[name] = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardEnglishName">English name of card</param>
        /// <returns></returns>
        public static float GetDiscoverValue(string cardEnglishName)
        {
            if (_discoverDictionary.ContainsKey(cardEnglishName))
            {
                var value = _discoverDictionary[cardEnglishName];
                return value;
            }
            else
            {
                Helpfunctions.Instance.ErrorLog(string.Format("Can not find the card \"{0}\" in discover dictionary.",cardEnglishName));
                return 0;
            }
        }
    }
}
