using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using log4net;
using Triton.Bot;
using Triton.Bot.Settings;
using Triton.Common;
using Triton.Game;
using Triton.Game.Mapping;
using Logger = Triton.Common.LogUtilities.Logger;

namespace StatsPlus
{
    public class StatsPlus : IPlugin
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private bool _enabled;

        private bool jsq = true;

        #region Implementation of IPlugin

        /// <summary> The name of the plugin. </summary>
        public string Name
        {
            get { return "统计增强版"; }
        }

        /// <summary> The description of the plugin. </summary>
        public string Description
        {
            get { return "记录对阵各职业胜负场、胜率."; }
        }

        /// <summary>The author of the plugin.</summary>
        public string Author
        {
            get { return "炉石兄弟"; }
        }

        /// <summary>The version of the plugin.</summary>
        public string Version
        {
            get { return "0.0.3.2"; }
        }

        /// <summary>Initializes this object. This is called when the object is loaded into the bot.</summary>
        public void Initialize()
        {
            Log.DebugFormat("[统计插件] 初始化");
            TritonHs.OnGuiTick += TritonHsOnOnGuiTick;
        }

        /// <summary> The plugin start callback. Do any initialization here. </summary>
        public void Start()
        {
            Log.DebugFormat("[统计插件] 开启");

            GameEventManager.GameOver += GameEventManagerOnGameOver;
            GameEventManager.StartingNewGame += GameEventManagerOnStartingNewGame;
        }

        /// <summary> The plugin tick callback. Do any update logic here. </summary>
        public void Tick()
        {
        }

        /// <summary> The plugin stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {
            Log.DebugFormat("[统计插件]停止");

            GameEventManager.GameOver -= GameEventManagerOnGameOver;
            GameEventManager.StartingNewGame -= GameEventManagerOnStartingNewGame;
        }

        public JsonSettings Settings
        {
            get { return StatsPlusSettings.Instance; }
        }

        private UserControl _control;

        /// <summary> The plugin's settings control. This will be added to the Hearthbuddy Settings tab.</summary>
        public UserControl Control
        {
            get
            {
                if (_control != null)
                {
                    return _control;
                }

                using (var fs = new FileStream(@"Plugins\StatsPlus\SettingsGui.xaml", FileMode.Open))
                {
                    var root = (UserControl)XamlReader.Load(fs);

                    // Your settings binding here.

                    if (!Wpf.SetupTextBoxBinding(root, "WinsTextBox", "Wins",
                        BindingMode.TwoWay, StatsPlusSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'WinsTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    Wpf.SetupTextBoxBinding(root, "WinsTextBox1", "Wins1", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "WinsTextBox2", "Wins2", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "WinsTextBox3", "Wins3", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "WinsTextBox4", "Wins4", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "WinsTextBox5", "Wins5", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "WinsTextBox6", "Wins6", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "WinsTextBox7", "Wins7", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "WinsTextBox8", "Wins8", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "WinsTextBox9", "Wins9", BindingMode.TwoWay, StatsPlusSettings.Instance);




                    if (!Wpf.SetupTextBoxBinding(root, "LossesTextBox", "Losses",
                        BindingMode.TwoWay, StatsPlusSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'LossesTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox1", "Losses1", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox2", "Losses2", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox3", "Losses3", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox4", "Losses4", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox5", "Losses5", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox6", "Losses6", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox7", "Losses7", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox8", "Losses8", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "LossesTextBox9", "Losses9", BindingMode.TwoWay, StatsPlusSettings.Instance);

                    if (!Wpf.SetupTextBoxBinding(root, "ConcedesTextBox", "Concedes",
                        BindingMode.TwoWay, StatsPlusSettings.Instance))
                    {
                        Log.DebugFormat("[SettingsControl] SetupTextBoxBinding failed for 'ConcedesTextBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox1", "Concedes1", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox2", "Concedes2", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox3", "Concedes3", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox4", "Concedes4", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox5", "Concedes5", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox6", "Concedes6", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox7", "Concedes7", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox8", "Concedes8", BindingMode.TwoWay, StatsPlusSettings.Instance);
                    Wpf.SetupTextBoxBinding(root, "ConcedesTextBox9", "Concedes9", BindingMode.TwoWay, StatsPlusSettings.Instance);


                    // Your settings event handlers here.

                    var resetButton = Wpf.FindControlByName<Button>(root, "ResetButton");
                    resetButton.Click += ResetButtonOnClick;
                    UpdateMainGuiStats();

                    _control = root;
                }

                return _control;
            }
        }

        /// <summary>Is this plugin currently enabled?</summary>
        public bool IsEnabled
        {
            get { return _enabled; }
        }

        /// <summary> The plugin is being enabled.</summary>
        public void Enable()
        {
            Log.DebugFormat("[统计插件] 启用");
            _enabled = true;
        }

        /// <summary> The plugin is being disabled.</summary>
        public void Disable()
        {
            Log.DebugFormat("[统计插件] 禁用");
            _enabled = false;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>Deinitializes this object. This is called when the object is being unloaded from the bot.</summary>
        public void Deinitialize()
        {
            TritonHs.OnGuiTick -= TritonHsOnOnGuiTick;
        }

        #endregion

        #region Override of Object

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + ": " + Description;
        }

        #endregion

        private void GameEventManagerOnStartingNewGame(object sender, StartingNewGameEventArgs e)
        {
            jsq = true;
        }

        private void GameEventManagerOnGameOver(object sender, GameOverEventArgs gameOverEventArgs)
        {

            if (!jsq) return;
            if (gameOverEventArgs.Result == GameOverFlag.Victory)
            {
                StatsPlusSettings.Instance.Wins++;
                StatsPlusSettings.Instance.Concedes = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins / (float)(StatsPlusSettings.Instance.Wins + StatsPlusSettings.Instance.Losses));
                if ((int)TritonHs.EnemyHero.Class == 10)//战士
                {
                    StatsPlusSettings.Instance.Wins1++;
                    StatsPlusSettings.Instance.Concedes1 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins1 / (float)(StatsPlusSettings.Instance.Wins1 + StatsPlusSettings.Instance.Losses1));
                }
                if ((int)TritonHs.EnemyHero.Class == 8)//萨满
                {
                    StatsPlusSettings.Instance.Wins2++;
                    StatsPlusSettings.Instance.Concedes2 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins2 / (float)(StatsPlusSettings.Instance.Wins2 + StatsPlusSettings.Instance.Losses2));
                }
                if ((int)TritonHs.EnemyHero.Class == 7)//盗贼
                {
                    StatsPlusSettings.Instance.Wins3++;
                    StatsPlusSettings.Instance.Concedes3 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins3 / (float)(StatsPlusSettings.Instance.Wins3 + StatsPlusSettings.Instance.Losses3));
                }
                if ((int)TritonHs.EnemyHero.Class == 5)//圣骑士
                {
                    StatsPlusSettings.Instance.Wins4++;
                    StatsPlusSettings.Instance.Concedes4 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins4 / (float)(StatsPlusSettings.Instance.Wins4 + StatsPlusSettings.Instance.Losses4));
                }
                if ((int)TritonHs.EnemyHero.Class == 3)//猎人
                {
                    StatsPlusSettings.Instance.Wins5++;
                    StatsPlusSettings.Instance.Concedes5 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins5 / (float)(StatsPlusSettings.Instance.Wins5 + StatsPlusSettings.Instance.Losses5));
                }
                if ((int)TritonHs.EnemyHero.Class == 2)//德鲁伊
                {
                    StatsPlusSettings.Instance.Wins6++;
                    StatsPlusSettings.Instance.Concedes6 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins6 / (float)(StatsPlusSettings.Instance.Wins6 + StatsPlusSettings.Instance.Losses6));
                }
                if ((int)TritonHs.EnemyHero.Class == 9)//术士
                {
                    StatsPlusSettings.Instance.Wins7++;
                    StatsPlusSettings.Instance.Concedes7 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins7 / (float)(StatsPlusSettings.Instance.Wins7 + StatsPlusSettings.Instance.Losses7));
                }
                if ((int)TritonHs.EnemyHero.Class == 4)//法师
                {
                    StatsPlusSettings.Instance.Wins8++;
                    StatsPlusSettings.Instance.Concedes8 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins8 / (float)(StatsPlusSettings.Instance.Wins8 + StatsPlusSettings.Instance.Losses8));
                }
                if ((int)TritonHs.EnemyHero.Class == 6)//牧师
                {
                    StatsPlusSettings.Instance.Wins9++;
                    StatsPlusSettings.Instance.Concedes9 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins9 / (float)(StatsPlusSettings.Instance.Wins9 + StatsPlusSettings.Instance.Losses9));
                }
                UpdateMainGuiStats();
            }
            else if (gameOverEventArgs.Result == GameOverFlag.Defeat)
            {
                StatsPlusSettings.Instance.Losses++;
                StatsPlusSettings.Instance.Concedes = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins / (float)(StatsPlusSettings.Instance.Wins + StatsPlusSettings.Instance.Losses));
                if ((int)TritonHs.EnemyHero.Class == 10)//战士
                {
                    StatsPlusSettings.Instance.Losses1++;
                    StatsPlusSettings.Instance.Concedes1 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins1 / (float)(StatsPlusSettings.Instance.Wins1 + StatsPlusSettings.Instance.Losses1));
                }
                if ((int)TritonHs.EnemyHero.Class == 8)//萨满
                {
                    StatsPlusSettings.Instance.Losses2++;
                    StatsPlusSettings.Instance.Concedes2 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins2 / (float)(StatsPlusSettings.Instance.Wins2 + StatsPlusSettings.Instance.Losses2));
                }
                if ((int)TritonHs.EnemyHero.Class == 7)//盗贼
                {
                    StatsPlusSettings.Instance.Losses3++;
                    StatsPlusSettings.Instance.Concedes3 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins3 / (float)(StatsPlusSettings.Instance.Wins3 + StatsPlusSettings.Instance.Losses3));
                }
                if ((int)TritonHs.EnemyHero.Class == 5)//圣骑士
                {
                    StatsPlusSettings.Instance.Losses4++;
                    StatsPlusSettings.Instance.Concedes4 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins4 / (float)(StatsPlusSettings.Instance.Wins4 + StatsPlusSettings.Instance.Losses4));
                }
                if ((int)TritonHs.EnemyHero.Class == 3)//猎人
                {
                    StatsPlusSettings.Instance.Losses5++;
                    StatsPlusSettings.Instance.Concedes5 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins5 / (float)(StatsPlusSettings.Instance.Wins5 + StatsPlusSettings.Instance.Losses5));
                }
                if ((int)TritonHs.EnemyHero.Class == 2)//德鲁伊
                {
                    StatsPlusSettings.Instance.Losses6++;
                    StatsPlusSettings.Instance.Concedes6 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins6 / (float)(StatsPlusSettings.Instance.Wins6 + StatsPlusSettings.Instance.Losses6));
                }
                if ((int)TritonHs.EnemyHero.Class == 9)//术士
                {
                    StatsPlusSettings.Instance.Losses7++;
                    StatsPlusSettings.Instance.Concedes7 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins7 / (float)(StatsPlusSettings.Instance.Wins7 + StatsPlusSettings.Instance.Losses7));
                }
                if ((int)TritonHs.EnemyHero.Class == 4)//法师
                {
                    StatsPlusSettings.Instance.Losses8++;
                    StatsPlusSettings.Instance.Concedes8 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins8 / (float)(StatsPlusSettings.Instance.Wins8 + StatsPlusSettings.Instance.Losses8));
                }
                if ((int)TritonHs.EnemyHero.Class == 6)//牧师
                {
                    StatsPlusSettings.Instance.Losses9++;
                    StatsPlusSettings.Instance.Concedes9 = string.Format("{0:0.0}%", 100.0f * (float)StatsPlusSettings.Instance.Wins9 / (float)(StatsPlusSettings.Instance.Wins9 + StatsPlusSettings.Instance.Losses9));
                }
                UpdateMainGuiStats();
            }
            jsq = false;
        }

        private void ResetButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            StatsPlusSettings.Instance.Wins = 0;
            StatsPlusSettings.Instance.Wins1 = 0;
            StatsPlusSettings.Instance.Wins2 = 0;
            StatsPlusSettings.Instance.Wins3 = 0;
            StatsPlusSettings.Instance.Wins4 = 0;
            StatsPlusSettings.Instance.Wins5 = 0;
            StatsPlusSettings.Instance.Wins6 = 0;
            StatsPlusSettings.Instance.Wins7 = 0;
            StatsPlusSettings.Instance.Wins8 = 0;
            StatsPlusSettings.Instance.Wins9 = 0;

            StatsPlusSettings.Instance.Losses = 0;
            StatsPlusSettings.Instance.Losses1 = 0;
            StatsPlusSettings.Instance.Losses2 = 0;
            StatsPlusSettings.Instance.Losses3 = 0;
            StatsPlusSettings.Instance.Losses4 = 0;
            StatsPlusSettings.Instance.Losses5 = 0;
            StatsPlusSettings.Instance.Losses6 = 0;
            StatsPlusSettings.Instance.Losses7 = 0;
            StatsPlusSettings.Instance.Losses8 = 0;
            StatsPlusSettings.Instance.Losses9 = 0;

            StatsPlusSettings.Instance.Concedes = "0.0%";
            StatsPlusSettings.Instance.Concedes1 = "0.0%";
            StatsPlusSettings.Instance.Concedes2 = "0.0%";
            StatsPlusSettings.Instance.Concedes3 = "0.0%";
            StatsPlusSettings.Instance.Concedes4 = "0.0%";
            StatsPlusSettings.Instance.Concedes5 = "0.0%";
            StatsPlusSettings.Instance.Concedes6 = "0.0%";
            StatsPlusSettings.Instance.Concedes7 = "0.0%";
            StatsPlusSettings.Instance.Concedes8 = "0.0%";
            StatsPlusSettings.Instance.Concedes9 = "0.0%";

            UpdateMainGuiStats();
        }

        private void UpdateMainGuiStats()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var leftControl = Wpf.FindControlByName<Label>(Application.Current.MainWindow, "StatusBarLeftLabel");
                var rightControl = Wpf.FindControlByName<Label>(Application.Current.MainWindow, "StatusBarRightLabel");

                if (StatsPlusSettings.Instance.Wins + StatsPlusSettings.Instance.Losses == 0)
                {
                    rightControl.Content = string.Format("(没有对局信息)");
                }
                else
                {
                    int sum = StatsPlusSettings.Instance.Wins + StatsPlusSettings.Instance.Losses;
                    rightControl.Content = string.Format("{0} 胜场/ {1} 总场数(胜率:{2:0.00} %) ",
                        StatsPlusSettings.Instance.Wins,
                        sum,
                        100.0f * (float)StatsPlusSettings.Instance.Wins / (float)(sum));
                    Log.InfoFormat("[统计插件] 合计: {0}", rightControl.Content);
                }

                // Force a save all.
                Configuration.Instance.SaveAll();
            }));
        }

        private void TritonHsOnOnGuiTick(object sender, GuiTickEventArgs guiTickEventArgs)
        {
            // Only update if we're actually enabled.
            if (IsEnabled)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var leftControl = Wpf.FindControlByName<Label>(Application.Current.MainWindow, "StatusBarLeftLabel");
                    leftControl.Content = string.Format("运行时间: {0}", TritonHs.Runtime.Elapsed.ToString("h'小时 'm'分 's'秒'"));
                }));
            }
        }
    }
}
