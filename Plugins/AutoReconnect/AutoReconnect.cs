using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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

namespace AutoReconnect
{
    public class AutoReconnect : IPlugin
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private bool _enabled;

        #region Implementation of IPlugin

        /// <summary> The name of the plugin. </summary>
        public string Name
        {
            get { return "自动重连"; }
        }

        /// <summary> The description of the plugin. </summary>
        public string Description
        {
            get { return "掉线时自动重连"; }
        }

        /// <summary>The author of the plugin.</summary>
        public string Author
        {
            get { return "wjhwjhn"; }
        }

        /// <summary>The version of the plugin.</summary>
        public string Version
        {
            get { return "2020-03-12 beta 0.1"; }
        }

        /// <summary>Initializes this object. This is called when the object is loaded into the bot.</summary>
        public void Initialize()
        {
            Log.DebugFormat("[自动重连插件] 初始化");
        }

        /// <summary> The plugin start callback. Do any initialization here. </summary>
        public void Start()
        {
            Log.DebugFormat("[自动重连插件] 开启");
        }

        /// <summary> The plugin tick callback. Do any update logic here. </summary>
        public void Tick()
        {
            
        }

        /// <summary> The plugin stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {            
            Log.DebugFormat("[自动重连插件]停止");
            DialogManager dialogManager = DialogManager.Get();
            if (dialogManager == null) return;
            if (!dialogManager.ShowingDialog()) return;

            DialogBase currentDialog = dialogManager.m_currentDialog;
            if (currentDialog == null)
            {
                Log.DebugFormat("[自动重连插件] Dialog 获取失败");
                return;
            }
            string realClassName = currentDialog.RealClassName;
            if (realClassName == "ReconnectHelperDialog")
            {
                Log.DebugFormat("[自动重连插件] 正在重新连接....");
                currentDialog.Hide();//not the best
                TritonHs.ClickCollectionButton(false);
                ThreadStart threadStart = new ThreadStart(ReStart);
                Thread thread = new Thread(threadStart);
                thread.Start();
            }
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
                using (var fs = new FileStream(@"Plugins\AutoReconnect\SettingsGui.xaml", FileMode.Open))
                {
                    var root = (UserControl)XamlReader.Load(fs);
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

        public JsonSettings Settings
        {
            get { return AutoReconnectSettings.Instance; }
        }

        /// <summary> The plugin is being enabled.</summary>
        public void Enable()
        {
            Log.DebugFormat("[自动重连插件] 启用");
            _enabled = true;
        }

        /// <summary> The plugin is being disabled.</summary>
        public void Disable()
        {
            Log.DebugFormat("[自动重连插件] 禁用");
            _enabled = false;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>Deinitializes this object. This is called when the object is being unloaded from the bot.</summary>
        public void Deinitialize()
        {

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

        public void ReStart()
        {
            //暂停20秒再运行
            Log.DebugFormat("[自动重连插件] 20秒后重新开始炉石兄弟");
            Thread.Sleep(20000);
            Log.DebugFormat("[自动重连插件] 正在开始炉石兄弟...");
            BotManager.Start();
        }
    }
}
