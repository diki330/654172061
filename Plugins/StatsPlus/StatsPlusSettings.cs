using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using log4net;
using Newtonsoft.Json;
using Triton.Bot.Settings;
using Triton.Common;
using Logger = Triton.Common.LogUtilities.Logger;

namespace StatsPlus
{
    /// <summary>Settings for the StatsPlus plugin. </summary>
    public class StatsPlusSettings : JsonSettings
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private static StatsPlusSettings _instance;

        /// <summary>The current instance for this class. </summary>
        public static StatsPlusSettings Instance
        {
            get { return _instance ?? (_instance = new StatsPlusSettings()); }
        }

        /// <summary>The default ctor. Will use the settings path "StatsPlus".</summary>
        public StatsPlusSettings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, string.Format("{0}.json", "StatsPlus")))
        {
        }

        private int _wins;
		private int _wins1;
		private int _wins2;
		private int _wins3;
		private int _wins4;
		private int _wins5;
		private int _wins6;
		private int _wins7;
		private int _wins8;
		private int _wins9;
        private int _losses;
        private int _losses1;
        private int _losses2;
        private int _losses3;
        private int _losses4;
        private int _losses5;
        private int _losses6;
        private int _losses7;
        private int _losses8;
        private int _losses9;		
        private string _concedes;
		private string _concedes1;
		private string _concedes2;
		private string _concedes3;
		private string _concedes4;
		private string _concedes5;
		private string _concedes6;
		private string _concedes7;
		private string _concedes8;
		private string _concedes9;
		

        /// <summary>Current stored wins.</summary>
        [DefaultValue(0)]
        public int Wins
        {
            get { return _wins; }
            set
            {
                if (value.Equals(_wins))
                {
                    return;
                }
                _wins = value;
                NotifyPropertyChanged(() => Wins);
            }
        }
		
        [DefaultValue(0)]
        public int Wins1
        {
            get { return _wins1; }
            set
            {
                if (value.Equals(_wins1))
                {
                    return;
                }
                _wins1 = value;
                NotifyPropertyChanged(() => Wins1);
            }
        }
        [DefaultValue(0)]
        public int Wins2
        {
            get { return _wins2; }
            set
            {
                if (value.Equals(_wins2))
                {
                    return;
                }
                _wins2 = value;
                NotifyPropertyChanged(() => Wins2);
            }
        }
        [DefaultValue(0)]
        public int Wins3
        {
            get { return _wins3; }
            set
            {
                if (value.Equals(_wins3))
                {
                    return;
                }
                _wins3 = value;
                NotifyPropertyChanged(() => Wins3);
            }
        }
        [DefaultValue(0)]
        public int Wins4
        {
            get { return _wins4; }
            set
            {
                if (value.Equals(_wins4))
                {
                    return;
                }
                _wins4 = value;
                NotifyPropertyChanged(() => Wins4);
            }
        }
        [DefaultValue(0)]
        public int Wins5
        {
            get { return _wins5; }
            set
            {
                if (value.Equals(_wins5))
                {
                    return;
                }
                _wins5 = value;
                NotifyPropertyChanged(() => Wins5);
            }
        }
        [DefaultValue(0)]
        public int Wins6
        {
            get { return _wins6; }
            set
            {
                if (value.Equals(_wins6))
                {
                    return;
                }
                _wins6 = value;
                NotifyPropertyChanged(() => Wins6);
            }
        }
        [DefaultValue(0)]
        public int Wins7
        {
            get { return _wins7; }
            set
            {
                if (value.Equals(_wins7))
                {
                    return;
                }
                _wins7 = value;
                NotifyPropertyChanged(() => Wins7);
            }
        }
        [DefaultValue(0)]
        public int Wins8
        {
            get { return _wins8; }
            set
            {
                if (value.Equals(_wins8))
                {
                    return;
                }
                _wins8 = value;
                NotifyPropertyChanged(() => Wins8);
            }
        }
        [DefaultValue(0)]
        public int Wins9
        {
            get { return _wins9; }
            set
            {
                if (value.Equals(_wins9))
                {
                    return;
                }
                _wins9 = value;
                NotifyPropertyChanged(() => Wins9);
            }
        }
		
        /// <summary>Current stored losses.</summary>
        [DefaultValue(0)]
        public int Losses
        {
            get { return _losses; }
            set
            {
                if (value.Equals(_losses))
                {
                    return;
                }
                _losses = value;
                NotifyPropertyChanged(() => Losses);
            }
        }
		
        [DefaultValue(0)]
        public int Losses1
        {
            get { return _losses1; }
            set
            {
                if (value.Equals(_losses1))
                {
                    return;
                }
                _losses1 = value;
                NotifyPropertyChanged(() => Losses1);
            }
        }
        [DefaultValue(0)]
        public int Losses2
        {
            get { return _losses2; }
            set
            {
                if (value.Equals(_losses2))
                {
                    return;
                }
                _losses2 = value;
                NotifyPropertyChanged(() => Losses2);
            }
        }
        [DefaultValue(0)]
        public int Losses3
        {
            get { return _losses3; }
            set
            {
                if (value.Equals(_losses3))
                {
                    return;
                }
                _losses3 = value;
                NotifyPropertyChanged(() => Losses3);
            }
        }
        [DefaultValue(0)]
        public int Losses4
        {
            get { return _losses4; }
            set
            {
                if (value.Equals(_losses4))
                {
                    return;
                }
                _losses4 = value;
                NotifyPropertyChanged(() => Losses4);
            }
        }
        [DefaultValue(0)]
        public int Losses5
        {
            get { return _losses5; }
            set
            {
                if (value.Equals(_losses5))
                {
                    return;
                }
                _losses5 = value;
                NotifyPropertyChanged(() => Losses5);
            }
        }
        [DefaultValue(0)]
        public int Losses6
        {
            get { return _losses6; }
            set
            {
                if (value.Equals(_losses6))
                {
                    return;
                }
                _losses6 = value;
                NotifyPropertyChanged(() => Losses6);
            }
        }
        [DefaultValue(0)]
        public int Losses7
        {
            get { return _losses7; }
            set
            {
                if (value.Equals(_losses7))
                {
                    return;
                }
                _losses7 = value;
                NotifyPropertyChanged(() => Losses7);
            }
        }
        [DefaultValue(0)]
        public int Losses8
        {
            get { return _losses8; }
            set
            {
                if (value.Equals(_losses8))
                {
                    return;
                }
                _losses8 = value;
                NotifyPropertyChanged(() => Losses8);
            }
        }
        [DefaultValue(0)]
        public int Losses9
        {
            get { return _losses9; }
            set
            {
                if (value.Equals(_losses9))
                {
                    return;
                }
                _losses9 = value;
                NotifyPropertyChanged(() => Losses9);
            }
        }		

        /// <summary>Current stored concedes.</summary>
        [DefaultValue("0.0%")]
        public string Concedes
        {
            get { return _concedes; }
            set
            {
                if (value.Equals(_concedes))
                {
                    return;
                }
                _concedes = value;
                NotifyPropertyChanged(() => Concedes);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes1
        {
            get { return _concedes1; }
            set
            {
                if (value.Equals(_concedes1))
                {
                    return;
                }
                _concedes1 = value;
                NotifyPropertyChanged(() => Concedes1);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes2
        {
            get { return _concedes2; }
            set
            {
                if (value.Equals(_concedes2))
                {
                    return;
                }
                _concedes2 = value;
                NotifyPropertyChanged(() => Concedes2);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes3
        {
            get { return _concedes3; }
            set
            {
                if (value.Equals(_concedes3))
                {
                    return;
                }
                _concedes3 = value;
                NotifyPropertyChanged(() => Concedes3);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes4
        {
            get { return _concedes4; }
            set
            {
                if (value.Equals(_concedes4))
                {
                    return;
                }
                _concedes4 = value;
                NotifyPropertyChanged(() => Concedes4);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes5
        {
            get { return _concedes5; }
            set
            {
                if (value.Equals(_concedes5))
                {
                    return;
                }
                _concedes5 = value;
                NotifyPropertyChanged(() => Concedes5);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes6
        {
            get { return _concedes6; }
            set
            {
                if (value.Equals(_concedes6))
                {
                    return;
                }
                _concedes6 = value;
                NotifyPropertyChanged(() => Concedes6);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes7
        {
            get { return _concedes7; }
            set
            {
                if (value.Equals(_concedes7))
                {
                    return;
                }
                _concedes7 = value;
                NotifyPropertyChanged(() => Concedes7);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes8
        {
            get { return _concedes8; }
            set
            {
                if (value.Equals(_concedes8))
                {
                    return;
                }
                _concedes8 = value;
                NotifyPropertyChanged(() => Concedes8);
            }
        }
		[DefaultValue("0.0%")]
        public string Concedes9
        {
            get { return _concedes9; }
            set
            {
                if (value.Equals(_concedes9))
                {
                    return;
                }
                _concedes9 = value;
                NotifyPropertyChanged(() => Concedes9);
            }
        }
		
    }
}
