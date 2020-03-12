using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using log4net;
using Newtonsoft.Json;
using Triton.Bot.Settings;
using Triton.Common;
using Triton.Game.Mapping;
using Logger = Triton.Common.LogUtilities.Logger;

namespace AutoReconnect
{
    /// <summary>Settings for the Stats plugin. </summary>
    public class AutoReconnectSettings : JsonSettings
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private static AutoReconnectSettings _instance;

        /// <summary>The current instance for this class. </summary>
        public static AutoReconnectSettings Instance
        {
            get { return _instance ?? (_instance = new AutoReconnectSettings()); }
        }

        /// <summary>The default ctor. Will use the settings path "Stats".</summary>
        public AutoReconnectSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, string.Format("{0}.json", "AutoReconnect")))
        {
        }
    }
}
