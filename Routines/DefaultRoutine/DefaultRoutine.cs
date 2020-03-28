using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using Buddy.Coroutines;
using HREngine.Bots;
using IronPython.Modules;
using log4net;
using Microsoft.Scripting.Hosting;
using Triton.Bot;
using Triton.Common;
using Triton.Game;
using Triton.Game.Data;

//!CompilerOption|AddRef|IronPython.dll
//!CompilerOption|AddRef|IronPython.Modules.dll
//!CompilerOption|AddRef|Microsoft.Scripting.dll
//!CompilerOption|AddRef|Microsoft.Dynamic.dll
//!CompilerOption|AddRef|Microsoft.Scripting.Metadata.dll
using Triton.Game.Mapping;
using Triton.Bot.Logic.Bots.DefaultBot;
using Logger = Triton.Common.LogUtilities.Logger;

namespace HREngine.Bots
{
    public class DefaultRoutine : IRoutine
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private readonly ScriptManager _scriptManager = new ScriptManager();

        private readonly List<Tuple<string, string>> _mulliganRules = new List<Tuple<string, string>>();

        private int dirtyTargetSource = -1;
        private int stopAfterWins = 30;
        private int concedeLvl = 5; // the rank, till you want to concede
        private int dirtytarget = -1;
        private int dirtychoice = -1;
        private string choiceCardId = "";
        DateTime starttime = DateTime.Now;
        bool enemyConcede = false;

        public bool learnmode = false;
        public bool printlearnmode = true;

        Silverfish sf = Silverfish.Instance;
        DefaultBotSettings botset
        {
            get { return DefaultBotSettings.Instance; }
        }
        //uncomment the desired option, or leave it as is to select via the interface
        Behavior behave = new BehaviorControl();
        //Behavior behave = new BehaviorRush();



        public DefaultRoutine()
        {
            // Global rules. Never keep a 4+ minion, unless it's Bolvar Fordragon (paladin).
            _mulliganRules.Add(new Tuple<string, string>("True", "card.Entity.Cost >= 4 and card.Entity.Id != \"GVG_063\""));

            // Never keep Tracking.
            _mulliganRules.Add(new Tuple<string, string>("mulliganData.UserClass == TAG_CLASS.HUNTER", "card.Entity.Id == \"DS1_184\""));

            // Example rule for self.
            //_mulliganRules.Add(new Tuple<string, string>("mulliganData.UserClass == TAG_CLASS.MAGE", "card.Cost >= 5"));

            // Example rule for opponents.
            //_mulliganRules.Add(new Tuple<string, string>("mulliganData.OpponentClass == TAG_CLASS.MAGE", "card.Cost >= 3"));

            // Example rule for matchups.
            //_mulliganRules.Add(new Tuple<string, string>("mulliganData.userClass == TAG_CLASS.HUNTER && mulliganData.OpponentClass == TAG_CLASS.DRUID", "card.Cost >= 2"));

            bool concede = false;
            bool teststuff = false;
            // set to true, to run a testfile (requires test.txt file in folder where _cardDB.txt file is located)
            bool printstuff = false; // if true, the best board of the tested file is printet stepp by stepp

            Helpfunctions.Instance.ErrorLog("----------------------------");
            Helpfunctions.Instance.ErrorLog("您正在使用的AI版本为" + Silverfish.Instance.versionnumber);
            Helpfunctions.Instance.ErrorLog("----------------------------");

            if (teststuff)
            {
                Ai.Instance.autoTester(printstuff);
            }
        }

        #region Scripting

        private const string BoilerPlateExecute = @"
import sys
sys.stdout=ioproxy

def Execute():
    return bool({0})";

        public delegate void RegisterScriptVariableDelegate(ScriptScope scope);

        public bool GetCondition(string expression, IEnumerable<RegisterScriptVariableDelegate> variables)
        {
            var code = string.Format(BoilerPlateExecute, expression);
            var scope = _scriptManager.Scope;
            var scriptSource = _scriptManager.Engine.CreateScriptSourceFromString(code);
            scope.SetVariable("ioproxy", _scriptManager.IoProxy);
            foreach (var variable in variables)
            {
                variable(scope);
            }
            scriptSource.Execute(scope);
            return scope.GetVariable<Func<bool>>("Execute")();
        }

        public bool VerifyCondition(string expression,
            IEnumerable<string> variables, out Exception ex)
        {
            ex = null;
            try
            {
                var code = string.Format(BoilerPlateExecute, expression);
                var scope = _scriptManager.Scope;
                var scriptSource = _scriptManager.Engine.CreateScriptSourceFromString(code);
                scope.SetVariable("ioproxy", _scriptManager.IoProxy);
                foreach (var variable in variables)
                {
                    scope.SetVariable(variable, new object());
                }
                scriptSource.Compile();
            }
            catch (Exception e)
            {
                ex = e;
                return false;
            }
            return true;
        }

        #endregion

        #region Implementation of IAuthored

        /// <summary> The name of the routine. </summary>
        public string Name
        {
            get { return "策略"; }
        }

        /// <summary> The description of the routine. </summary>
        public string Description
        {
            get { return "炉石兄弟的默认策略."; }
        }

        /// <summary>The author of this routine.</summary>
        public string Author
        {
            get { return "Bossland GmbH"; }
        }

        /// <summary>The version of this routine.</summary>
        public string Version
        {
            get { return "0.0.1.1"; }
        }

        #endregion

        #region Implementation of IBase

        /// <summary>Initializes this routine.</summary>
        public void Initialize()
        {
            _scriptManager.Initialize(null,
                new List<string>
                {
                    "Triton.Game",
                    "Triton.Bot",
                    "Triton.Common",
                    "Triton.Game.Mapping",
                    "Triton.Game.Abstraction"
                });
        }

        /// <summary>Deinitializes this routine.</summary>
        public void Deinitialize()
        {
            _scriptManager.Deinitialize();
        }

        #endregion

        #region Implementation of IRunnable

        /// <summary> The routine start callback. Do any initialization here. </summary>
        public void Start()
        {
            GameEventManager.NewGame += GameEventManagerOnNewGame;
            GameEventManager.GameOver += GameEventManagerOnGameOver;
            GameEventManager.QuestUpdate += GameEventManagerOnQuestUpdate;
            GameEventManager.ArenaRewards += GameEventManagerOnArenaRewards;

            if (Hrtprozis.Instance.settings == null)
            {
                Hrtprozis.Instance.setInstances();
                ComboBreaker.Instance.setInstances();
                PenalityManager.Instance.setInstances();
            }
            behave = sf.getBehaviorByName(DefaultRoutineSettings.Instance.DefaultBehavior);
            foreach (var tuple in _mulliganRules)
            {
                Exception ex;
                if (
                    !VerifyCondition(tuple.Item1, new List<string> { "mulliganData" }, out ex))
                {
                    Log.ErrorFormat("[开始] 发现一个错误的留牌策略为 [{1}]: {0}.", ex,
                        tuple.Item1);
                    BotManager.Stop();
                }

                if (
                    !VerifyCondition(tuple.Item2, new List<string> { "mulliganData", "card" },
                        out ex))
                {
                    Log.ErrorFormat("[开始] 发现一个错误的留牌策略为 [{1}]: {0}.", ex,
                        tuple.Item2);
                    BotManager.Stop();
                }
            }
        }

        /// <summary> The routine tick callback. Do any update logic here. </summary>
        public void Tick()
        {
        }

        /// <summary> The routine stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {
            GameEventManager.NewGame -= GameEventManagerOnNewGame;
            GameEventManager.GameOver -= GameEventManagerOnGameOver;
            GameEventManager.QuestUpdate -= GameEventManagerOnQuestUpdate;
            GameEventManager.ArenaRewards -= GameEventManagerOnArenaRewards;
        }

        #endregion

        #region Implementation of IConfigurable

        /// <summary> The routine's settings control. This will be added to the Hearthbuddy Settings tab.</summary>
        public UserControl Control
        {
            get
            {
                using (var fs = new FileStream(@"Routines\DefaultRoutine\SettingsGui.xaml", FileMode.Open))
                {
                    var root = (UserControl)XamlReader.Load(fs);

                    // Your settings binding here.

                    // ArenaPreferredClass1
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass1ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass1ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass1ComboBox",
                            "ArenaPreferredClass1", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass1ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // ArenaPreferredClass2
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass2ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass2ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass2ComboBox",
                            "ArenaPreferredClass2", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass2ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // ArenaPreferredClass3
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass3ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass3ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass3ComboBox",
                            "ArenaPreferredClass3", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass3ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // ArenaPreferredClass4
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass4ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass4ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass4ComboBox",
                            "ArenaPreferredClass4", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass4ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // ArenaPreferredClass5
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "ArenaPreferredClass5ComboBox", "AllClasses",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'ArenaPreferredClass5ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "ArenaPreferredClass5ComboBox",
                            "ArenaPreferredClass5", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'ArenaPreferredClass5ComboBox'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    // defaultBehaviorComboBox1
                    if (
                        !Wpf.SetupComboBoxItemsBinding(root, "defaultBehaviorComboBox1", "AllBehav",
                            BindingMode.OneWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxItemsBinding failed for 'defaultBehaviorComboBox1'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }

                    if (
                        !Wpf.SetupComboBoxSelectedItemBinding(root, "defaultBehaviorComboBox1",
                            "DefaultBehavior", BindingMode.TwoWay, DefaultRoutineSettings.Instance))
                    {
                        Log.DebugFormat(
                            "[SettingsControl] SetupComboBoxSelectedItemBinding failed for 'defaultBehaviorComboBox1'.");
                        throw new Exception("The SettingsControl could not be created.");
                    }
                    // Your settings event handlers here.

                    return root;
                }
            }
        }

        /// <summary>The settings object. This will be registered in the current configuration.</summary>
        public JsonSettings Settings
        {
            get { return DefaultRoutineSettings.Instance; }
        }

        #endregion

        #region Implementation of IRoutine

        /// <summary>
        /// Sends data to the routine with the associated name.
        /// </summary>
        /// <param name="name">The name of the configuration.</param>
        /// <param name="param">The data passed for the configuration.</param>
        public void SetConfiguration(string name, params object[] param)
        {
        }

        /// <summary>
        /// Requests data from the routine with the associated name.
        /// </summary>
        /// <param name="name">The name of the configuration.</param>
        /// <returns>Data from the routine.</returns>
        public object GetConfiguration(string name)
        {
            return null;
        }

        /// <summary>
        /// The routine's coroutine logic to execute.
        /// </summary>
        /// <param name="type">The requested type of logic to execute.</param>
        /// <param name="context">Data sent to the routine from the bot for the current logic.</param>
        /// <returns>true if logic was executed to handle this type and false otherwise.</returns>
        public async Task<bool> Logic(string type, object context)
        {


            // The bot is requesting mulligan logic.
            if (type == "mulligan")
            {
                await MulliganLogic(context as MulliganData);
                return true;
            }

            // The bot is requesting emote logic.
            if (type == "emote")
            {
                await EmoteLogic(context as EmoteData);
                return true;
            }

            // The bot is requesting our turn logic.
            if (type == "our_turn")
            {
                await OurTurnLogic();
                return true;
            }

            // The bot is requesting opponent turn logic.
            if (type == "opponent_turn")
            {
                await OpponentTurnLogic();
                return true;
            }

            // The bot is requesting our turn logic.
            if (type == "our_turn_combat")
            {
                await OurTurnCombatLogic();
                return true;
            }

            // The bot is requesting opponent turn logic.
            if (type == "opponent_turn_combat")
            {
                await OpponentTurnCombatLogic();
                return true;
            }

            // The bot is requesting arena draft logic.
            if (type == "arena_draft")
            {
                await ArenaDraftLogic(context as ArenaDraftData);
                return true;
            }

            // The bot is requesting quest handling logic.
            if (type == "handle_quests")
            {
                await HandleQuestsLogic(context as QuestData);
                return true;
            }

            // Whatever the current logic type is, this routine doesn't implement it.
            return false;
        }

        #region Mulligan

        private int RandomMulliganThinkTime()
        {
            var random = Client.Random;
            var type = random.Next(0, 100) % 4;

            if (type == 0) return random.Next(200, 300);
            if (type == 1) return random.Next(300, 600);
            if (type == 2) return random.Next(600, 900);
            return 0;
        }

        /// <summary>
        /// This task implements custom mulligan choosing logic for the bot.
        /// The user is expected to set the Mulligans list elements to true/false 
        /// to signal to the bot which cards should/shouldn't be mulliganed. 
        /// This task should also implement humanization factors, such as hovering 
        /// over cards, or delaying randomly before returning, as the mulligan 
        /// process takes place as soon as the task completes.
        /// </summary>
        /// <param name="mulliganData">An object that contains relevant data for the mulligan process.</param>
        /// <returns></returns>
        public async Task MulliganLogic(MulliganData mulliganData)
        {
            if (!botset.AutoConcedeLag && !botset.ForceConcedeAtMulligan)
            {
                Log.InfoFormat("[日志档案:] 开始创建");
                Hrtprozis prozis = Hrtprozis.Instance;
                prozis.clearAllNewGame();
                Silverfish.Instance.setnewLoggFile();
                Log.InfoFormat("[日志档案:] 创建完成");
            }
            Log.InfoFormat("[开局留牌] {0} 对阵 {1}.", mulliganData.UserClass, mulliganData.OpponentClass);
            var count = mulliganData.Cards.Count;

            if (this.behave.BehaviorName() != DefaultRoutineSettings.Instance.DefaultBehavior)
            {
                behave = sf.getBehaviorByName(DefaultRoutineSettings.Instance.DefaultBehavior);
            }
            if (!Mulligan.Instance.getHoldList(mulliganData, this.behave))
            {
                for (var i = 0; i < count; i++)
                {
                    var card = mulliganData.Cards[i];

                    try
                    {
                        foreach (var tuple in _mulliganRules)
                        {
                            if (GetCondition(tuple.Item1,
                                new List<RegisterScriptVariableDelegate>
                            {
                                scope => scope.SetVariable("mulliganData", mulliganData)
                            }))
                            {
                                if (GetCondition(tuple.Item2,
                                    new List<RegisterScriptVariableDelegate>
                                {
                                    scope => scope.SetVariable("mulliganData", mulliganData),
                                    scope => scope.SetVariable("card", card)
                                }))
                                {
                                    mulliganData.Mulligans[i] = true;
                                    Log.InfoFormat(
                                        "[开局留牌] {0} 这张卡片符合自定义留牌规则: [{1}] ({2}).",
                                        card.Entity.Id, tuple.Item2, tuple.Item1);
                                }
                            }
                            else
                            {
                                Log.InfoFormat(
                                    "[开局留牌] 留牌策略检测发现 [{0}] 的规则错误, 所以 [{1}] 的规则不执行.",
                                    tuple.Item1, tuple.Item2);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat("[Mulligan] An exception occurred: {0}.", ex);
                        BotManager.Stop();
                        return;
                    }
                }
            }

            var thinkList = new List<KeyValuePair<int, int>>();
            for (var i = 0; i < count; i++)
            {
                thinkList.Add(new KeyValuePair<int, int>(i % count, RandomMulliganThinkTime()));
            }
            thinkList.Shuffle();

            foreach (var entry in thinkList)
            {
                var card = mulliganData.Cards[entry.Key];

                Log.InfoFormat("[开局留牌] 现在开始思考留牌 {0} 时间已经过去 {1} 毫秒.", card.Entity.Id, entry.Value);

                // Instant think time, skip the card.
                if (entry.Value == 0)
                    continue;

                Client.MouseOver(card.InteractPoint);

                await Coroutine.Sleep(entry.Value);
            }
        }

        #endregion

        #region Emote

        /// <summary>
        /// This task implements player emote detection logic for the bot.
        /// </summary>
        /// <param name="data">An object that contains relevant data for the emote event.</param>
        /// <returns></returns>
        public async Task EmoteLogic(EmoteData data)
        {
            Log.InfoFormat("[表情] 使用表情 [{0}].", data.Emote);

            if (data.Emote == EmoteType.GREETINGS)
            {
            }
            else if (data.Emote == EmoteType.WELL_PLAYED)
            {
            }
            else if (data.Emote == EmoteType.OOPS)
            {
            }
            else if (data.Emote == EmoteType.THREATEN)
            {
            }
            else if (data.Emote == EmoteType.THANKS)
            {
            }
            else if (data.Emote == EmoteType.SORRY)
            {
            }
        }

        #endregion

        #region Turn

        public async Task OurTurnCombatLogic()
        {
            Log.InfoFormat("[我方回合]");
            await Coroutine.Sleep(555 + makeChoice());
            if (dirtychoice != -1)
            {
                Client.LeftClickAt(Client.CardInteractPoint(ChoiceCardMgr.Get().GetFriendlyCards()[dirtychoice]));
            }
            dirtychoice = -1;
            await Coroutine.Sleep(555);
            Silverfish.Instance.lastpf = null;
            return;
        }

        public async Task OpponentTurnCombatLogic()
        {
            Log.Info("[对手回合]");
        }

        /// <summary>
        /// Under construction.
        /// </summary>
        /// <returns></returns>
        public async Task OurTurnLogic()
        {
            if (this.behave.BehaviorName() != DefaultRoutineSettings.Instance.DefaultBehavior)
            {
                behave = sf.getBehaviorByName(DefaultRoutineSettings.Instance.DefaultBehavior);
                Silverfish.Instance.lastpf = null;
            }

            if (this.learnmode && (TritonHs.IsInTargetMode() || TritonHs.IsInChoiceMode()))
            {
                await Coroutine.Sleep(50);
                return;
            }

            if (TritonHs.IsInTargetMode())
            {
                if (dirtytarget >= 0)
                {
                    Log.Info("瞄准中...");
                    HSCard source = null;
                    if (dirtyTargetSource == 9000) // 9000 = ability
                    {
                        source = TritonHs.OurHeroPowerCard;
                    }
                    else
                    {
                        source = getEntityWithNumber(dirtyTargetSource);
                    }
                    HSCard target = getEntityWithNumber(dirtytarget);

                    if (target == null)
                    {
                        Log.Error("目标为空...");
                        TritonHs.CancelTargetingMode();
                        return;
                    }

                    dirtytarget = -1;
                    dirtyTargetSource = -1;

                    if (source == null) await TritonHs.DoTarget(target);
                    else await source.DoTarget(target);

                    await Coroutine.Sleep(555);

                    return;
                }

                Log.Error("目标丢失...");
                TritonHs.CancelTargetingMode();
                return;
            }

            if (TritonHs.IsInChoiceMode())
            {
                await Coroutine.Sleep(555 + makeChoice());
                if (dirtychoice != -1)
                {
                    Client.LeftClickAt(Client.CardInteractPoint(ChoiceCardMgr.Get().GetFriendlyCards()[dirtychoice]));
                }
                dirtychoice = -1;
                await Coroutine.Sleep(555);
                return;
            }

            bool sleepRetry = false;
            bool templearn = Silverfish.Instance.updateEverything(behave, 0, out sleepRetry);
            if (sleepRetry)
            {
                Log.Error("[AI] 随从没能动起来，再试一次...");
                await Coroutine.Sleep(500);
                templearn = Silverfish.Instance.updateEverything(behave, 1, out sleepRetry);
            }

            if (templearn == true) this.printlearnmode = true;

            if (this.learnmode)
            {
                if (this.printlearnmode)
                {
                    Ai.Instance.simmulateWholeTurnandPrint();
                }
                this.printlearnmode = false;

                //do nothing
                await Coroutine.Sleep(50);
                return;
            }

            var moveTodo = Ai.Instance.bestmove;
            if (moveTodo == null || moveTodo.actionType == actionEnum.endturn || Ai.Instance.bestmoveValue < -9999)
            {
                bool doEndTurn = false;
                bool doConcede = false;
                if (Ai.Instance.bestmoveValue > -10000) doEndTurn = true;
                else if (HREngine.Bots.Settings.Instance.concedeMode != 0) doConcede = true;
                else
                {
                    if (new Playfield().ownHeroHasDirectLethal())
                    {
                        Playfield lastChancePl = Ai.Instance.bestplay;
                        bool lastChance = false;
                        if (lastChancePl.owncarddraw > 0)
                        {
                            foreach (Handmanager.Handcard hc in lastChancePl.owncards)
                            {
                                if (hc.card.name == CardDB.cardName.unknown) lastChance = true;
                            }
                            if (!lastChance) doConcede = true;
                        }
                        else doConcede = true;

                        if (doConcede)
                        {
                            foreach (Minion m in lastChancePl.ownMinions)
                            {
                                if (!m.playedThisTurn) continue;
                                switch (m.handcard.card.name)
                                {
                                    case CardDB.cardName.cthun: lastChance = true; break;
                                    case CardDB.cardName.nzoththecorruptor: lastChance = true; break;
                                    case CardDB.cardName.yoggsaronhopesend: lastChance = true; break;
                                    case CardDB.cardName.sirfinleymrrgglton: lastChance = true; break;
                                    case CardDB.cardName.ragnarosthefirelord: if (lastChancePl.enemyHero.Hp < 9) lastChance = true; break;
                                    case CardDB.cardName.barongeddon: if (lastChancePl.enemyHero.Hp < 3) lastChance = true; break;
                                }
                            }
                        }
                        if (lastChance) doConcede = false;
                    }
                    else if (moveTodo == null || moveTodo.actionType == actionEnum.endturn) doEndTurn = true;
                }
                if (doEndTurn)
                {
                    Helpfunctions.Instance.ErrorLog("回合结束");
                    await TritonHs.EndTurn();
                    return;
                }
                else if (doConcede)
                {
                    Helpfunctions.Instance.ErrorLog("我方败局已定. 投降...");
                    Helpfunctions.Instance.logg("投降... 败局已定###############################################");
                    TritonHs.Concede(true);
                    return;
                }
            }
            Helpfunctions.Instance.ErrorLog("开始行动");
            if (moveTodo == null)
            {
                Helpfunctions.Instance.ErrorLog("实在支不出招啦. 结束当前回合");
                await TritonHs.EndTurn();
                return;
            }

            //play the move#########################################################################

            {
                moveTodo.print();

                //play a card form hand
                if (moveTodo.actionType == actionEnum.playcard)
                {
                    Questmanager.Instance.updatePlayedCardFromHand(moveTodo.card);
                    HSCard cardtoplay = getCardWithNumber(moveTodo.card.entity);
                    if (cardtoplay == null)
                    {
                        Helpfunctions.Instance.ErrorLog("[提示] 实在支不出招啦");
                        return;
                    }
                    if (moveTodo.target != null)
                    {
                        HSCard target = getEntityWithNumber(moveTodo.target.entitiyID);
                        if (target != null)
                        {
                            Helpfunctions.Instance.ErrorLog("使用: " + cardtoplay.Name + " (" + cardtoplay.EntityId + ") 瞄准: " + target.Name + " (" + target.EntityId + ")");
                            Helpfunctions.Instance.logg("使用: " + cardtoplay.Name + " (" + cardtoplay.EntityId + ") 瞄准: " + target.Name + " (" + target.EntityId + ") 抉择: " + moveTodo.druidchoice);
                            if (moveTodo.druidchoice >= 1)
                            {
                                dirtytarget = moveTodo.target.entitiyID;
                                dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                                choiceCardId = moveTodo.card.card.cardIDenum.ToString();
                            }

                            //safe targeting stuff for hsbuddy
                            dirtyTargetSource = moveTodo.card.entity;
                            dirtytarget = moveTodo.target.entitiyID;

                            await cardtoplay.Pickup();

                            if (moveTodo.card.card.type == CardDB.cardtype.MOB)
                            {
                                await cardtoplay.UseAt(moveTodo.place);
                            }
                            else if (moveTodo.card.card.type == CardDB.cardtype.WEAPON) // This fixes perdition's blade
                            {
                                await cardtoplay.UseOn(target.Card);
                            }
                            else if (moveTodo.card.card.type == CardDB.cardtype.SPELL)
                            {
                                await cardtoplay.UseOn(target.Card);
                            }
                            else
                            {
                                await cardtoplay.UseOn(target.Card);
                            }
                        }
                        else
                        {
                            Helpfunctions.Instance.ErrorLog("[AI] 目标丢失，再试一次...");
                            Helpfunctions.Instance.logg("[AI] 目标 " + moveTodo.target.entitiyID + "丢失. 再试一次...");
                        }
                        await Coroutine.Sleep(500);

                        return;
                    }

                    Helpfunctions.Instance.ErrorLog("使用: " + cardtoplay.Name + " (" + cardtoplay.EntityId + ") 暂时没有目标");
                    Helpfunctions.Instance.logg("使用: " + cardtoplay.Name + " (" + cardtoplay.EntityId + ") 抉择: " + moveTodo.druidchoice);
                    if (moveTodo.druidchoice >= 1)
                    {
                        dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                        choiceCardId = moveTodo.card.card.cardIDenum.ToString();
                    }

                    dirtyTargetSource = -1;
                    dirtytarget = -1;

                    await cardtoplay.Pickup();

                    if (moveTodo.card.card.type == CardDB.cardtype.MOB)
                    {
                        await cardtoplay.UseAt(moveTodo.place);
                    }
                    else
                    {
                        await cardtoplay.Use();
                    }
                    await Coroutine.Sleep(500);

                    return;
                }

                //attack with minion
                if (moveTodo.actionType == actionEnum.attackWithMinion)
                {
                    HSCard attacker = getEntityWithNumber(moveTodo.own.entitiyID);
                    HSCard target = getEntityWithNumber(moveTodo.target.entitiyID);
                    if (attacker != null)
                    {
                        if (target != null)
                        {
                            Helpfunctions.Instance.ErrorLog("随从攻击: " + attacker.Name + " 目标为: " + target.Name);
                            Helpfunctions.Instance.logg("随从攻击: " + attacker.Name + " 目标为: " + target.Name);


                            await attacker.DoAttack(target);

                        }
                        else
                        {
                            Helpfunctions.Instance.ErrorLog("[AI] 目标丢失，再次重试...");
                            Helpfunctions.Instance.logg("[AI] 目标 " + moveTodo.target.entitiyID + "丢失. 再次重试...");
                        }
                    }
                    else
                    {
                        Helpfunctions.Instance.ErrorLog("[AI] 攻击失败，再次重试...");
                        Helpfunctions.Instance.logg("[AI] 进攻 " + moveTodo.own.entitiyID + " 失败.再次重试...");
                    }
                    await Coroutine.Sleep(250);
                    return;
                }
                //attack with hero
                if (moveTodo.actionType == actionEnum.attackWithHero)
                {
                    HSCard attacker = getEntityWithNumber(moveTodo.own.entitiyID);
                    HSCard target = getEntityWithNumber(moveTodo.target.entitiyID);
                    if (attacker != null)
                    {
                        if (target != null)
                        {
                            dirtytarget = moveTodo.target.entitiyID;
                            Helpfunctions.Instance.ErrorLog("英雄攻击: " + attacker.Name + " 目标为: " + target.Name);
                            Helpfunctions.Instance.logg("英雄攻击: " + attacker.Name + " 目标为: " + target.Name);

                            //safe targeting stuff for hsbuddy
                            dirtyTargetSource = moveTodo.own.entitiyID;
                            dirtytarget = moveTodo.target.entitiyID;
                            await attacker.DoAttack(target);
                        }
                        else
                        {
                            Helpfunctions.Instance.ErrorLog("[AI] 英雄攻击目标丢失，再次重试...");
                            Helpfunctions.Instance.logg("[AI] 英雄攻击目标 " + moveTodo.target.entitiyID + "丢失，再次重试...");
                        }
                    }
                    else
                    {
                        Helpfunctions.Instance.ErrorLog("[AI] 英雄攻击失败，再次重试...");
                        Helpfunctions.Instance.logg("[AI] 英雄攻击 " + moveTodo.own.entitiyID + " 失败，再次重试...");
                    }
                    await Coroutine.Sleep(250);
                    return;
                }

                //use ability
                if (moveTodo.actionType == actionEnum.useHeroPower)
                {
                    HSCard cardtoplay = TritonHs.OurHeroPowerCard;

                    if (moveTodo.target != null)
                    {
                        HSCard target = getEntityWithNumber(moveTodo.target.entitiyID);
                        if (target != null)
                        {
                            Helpfunctions.Instance.ErrorLog("使用英雄技能: " + cardtoplay.Name + " 目标为 " + target.Name);
                            Helpfunctions.Instance.logg("使用英雄技能: " + cardtoplay.Name + " 目标为 " + target.Name + (moveTodo.druidchoice > 0 ? (" 抉择: " + moveTodo.druidchoice) : ""));
                            if (moveTodo.druidchoice > 0)
                            {
                                dirtytarget = moveTodo.target.entitiyID;
                                dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                                choiceCardId = moveTodo.card.card.cardIDenum.ToString();
                            }

                            dirtyTargetSource = 9000;
                            dirtytarget = moveTodo.target.entitiyID;

                            await cardtoplay.Pickup();
                            await cardtoplay.UseOn(target.Card);
                        }
                        else
                        {
                            Helpfunctions.Instance.ErrorLog("[AI] 目标丢失，再次重试...");
                            Helpfunctions.Instance.logg("[AI] 目标 " + moveTodo.target.entitiyID + "丢失. 再次重试...");
                        }
                        await Coroutine.Sleep(500);
                    }
                    else
                    {
                        Helpfunctions.Instance.ErrorLog("使用英雄技能: " + cardtoplay.Name + " 暂时没有目标");
                        Helpfunctions.Instance.logg("使用英雄技能: " + cardtoplay.Name + " 暂时没有目标" + (moveTodo.druidchoice > 0 ? (" 抉择: " + moveTodo.druidchoice) : ""));

                        if (moveTodo.druidchoice >= 1)
                        {
                            dirtychoice = moveTodo.druidchoice; //1=leftcard, 2= rightcard
                            choiceCardId = moveTodo.card.card.cardIDenum.ToString();
                        }

                        dirtyTargetSource = -1;
                        dirtytarget = -1;

                        await cardtoplay.Pickup();
                    }

                    return;
                }
            }

            await TritonHs.EndTurn();
        }

        private int makeChoice()//发现机制
        {
            if (dirtychoice < 1)
            {
                var ccm = ChoiceCardMgr.Get();
                var lscc = ccm.m_lastShownChoiceState;
                GAME_TAG choiceMode = GAME_TAG.DISCOVER;
                int sourceEntityId = -1;
                CardDB.cardIDEnum sourceEntityCId = CardDB.cardIDEnum.None;
                if (lscc != null)
                {
                    sourceEntityId = lscc.m_sourceEntityId;
                    Entity entity = GameState.Get().GetEntity(lscc.m_sourceEntityId);
                    sourceEntityCId = CardDB.Instance.cardIdstringToEnum(entity.GetCardId());
                    if (entity != null)
                    {
                        var sourceCard = entity.GetCard();
                        if (sourceCard != null)
                        {
                            if (sourceCard.GetEntity().GetTag(GAME_TAG.DISCOVER) == 1)
                            {
                                choiceMode = GAME_TAG.DISCOVER;
                                dirtychoice = -1;
                            }
                            else if (sourceCard.GetEntity().HasReferencedTag(GAME_TAG.ADAPT))
                            {
                                choiceMode = GAME_TAG.ADAPT;
                                dirtychoice = -1;
                            }
                        }
                    }
                }

                Ai ai = Ai.Instance;
                List<Handmanager.Handcard> discoverCards = new List<Handmanager.Handcard>();
                float bestDiscoverValue = -2000000;
                var choiceCardMgr = ChoiceCardMgr.Get();
                var cards = choiceCardMgr.GetFriendlyCards();

                for (int i = 0; i < cards.Count; i++)
                {
                    var hc = new Handmanager.Handcard();
                    hc.card = CardDB.Instance.getCardDataFromID(CardDB.Instance.cardIdstringToEnum(cards[i].GetCardId()));
                    hc.position = 100 + i;
                    hc.entity = cards[i].GetEntityId();
                    hc.manacost = hc.card.calculateManaCost(ai.nextMoveGuess);
                    discoverCards.Add(hc);
                }

                int sirFinleyChoice = -1;
                if (ai.bestmove == null) Log.ErrorFormat("[提示] 没有获得卡牌数据");
                else if (ai.bestmove.actionType == actionEnum.playcard && ai.bestmove.card.card.name == CardDB.cardName.sirfinleymrrgglton)
                {
                    sirFinleyChoice = ai.botBase.getSirFinleyPriority(discoverCards);
                }
                if (choiceMode != GAME_TAG.DISCOVER) sirFinleyChoice = -1;

                DateTime tmp = DateTime.Now;
                int discoverCardsCount = discoverCards.Count;
                if (sirFinleyChoice != -1) dirtychoice = sirFinleyChoice;
                else
                {
                    int dirtyTwoTurnSim = ai.mainTurnSimulator.getSecondTurnSimu();
                    ai.mainTurnSimulator.setSecondTurnSimu(true, 50);
                    using (TritonHs.Memory.ReleaseFrame(true))
                    {
                        Playfield testPl = new Playfield();
                        Playfield basePlf = new Playfield(ai.nextMoveGuess);
                        for (int i = 0; i < discoverCardsCount; i++)
                        {
                            Playfield tmpPlf = new Playfield(basePlf);
                            tmpPlf.isLethalCheck = false;
                            float bestval = bestDiscoverValue;
                            switch (choiceMode)
                            {
                                case GAME_TAG.DISCOVER:
                                    switch (ai.bestmove.card.card.name)
                                    {
                                        case CardDB.cardName.eternalservitude:
                                        case CardDB.cardName.freefromamber:
                                            Minion m = tmpPlf.createNewMinion(discoverCards[i], tmpPlf.ownMinions.Count, true);
                                            tmpPlf.ownMinions[tmpPlf.ownMinions.Count - 1] = m;
                                            break;
                                        default:
                                            tmpPlf.owncards[tmpPlf.owncards.Count - 1] = discoverCards[i];
                                            break;
                                    }
                                    bestval = ai.mainTurnSimulator.doallmoves(tmpPlf);
                                    //发现卡牌价值
                                    if (discoverCards[i].card.name == CardDB.cardName.aberrantberserker) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.abominablebowman) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.abomination) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.abusivesergeant) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.abyssalenforcer) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.abyssalsummoner) bestval += 64;
                                    if (discoverCards[i].card.name == CardDB.cardName.academicespionage) bestval += 56;
                                    if (discoverCards[i].card.name == CardDB.cardName.acherusveteran) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.acidicswampooze) bestval += 104;
                                    if (discoverCards[i].card.name == CardDB.cardName.acolyteofagony) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.acolyteofpain) bestval += 417;
                                    if (discoverCards[i].card.name == CardDB.cardName.acornbearer) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.activatetheobelisk) bestval += 69;
                                    if (discoverCards[i].card.name == CardDB.cardName.adaptation) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.addledgrizzly) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.aeonreaver) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.aeroponics) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.airelemental) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.airraid) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.alakirthewindlord) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.alarmobot) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.aldorpeacekeeper) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.alexstrasza) bestval += 363;
                                    if (discoverCards[i].card.name == CardDB.cardName.alexstraszaschampion) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.alightinthedarkness) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.alleyarmorsmith) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.alleycat) bestval += 43;
                                    if (discoverCards[i].card.name == CardDB.cardName.aluneth) bestval += 368;
                                    if (discoverCards[i].card.name == CardDB.cardName.amaniberserker) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.amaniwarbear) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.amberwatcher) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancestorscall) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancestralguardian) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancestralhealing) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancestralknowledge) bestval += 54;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancestralspirit) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancharrr) bestval += 91;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientbrewmaster) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientharbinger) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientmage) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientmysteries) bestval += 444;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientofblossoms) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientoflore) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientofwar) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientshade) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientshieldbearer) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.ancientwatcher) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.anewchallenger) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.angrychicken) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.animagolem) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.animalcompanion) bestval += 172;
                                    if (discoverCards[i].card.name == CardDB.cardName.animatedarmor) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.animatedavalanche) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.animatedberserker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.annoyomodule) bestval += 169;
                                    if (discoverCards[i].card.name == CardDB.cardName.annoyotron) bestval += 181;
                                    if (discoverCards[i].card.name == CardDB.cardName.anodizedrobocub) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.anomalus) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.antiquehealbot) bestval += 349;
                                    if (discoverCards[i].card.name == CardDB.cardName.anubarak) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.anubarambusher) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.anubisathdefender) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.anubisathsentinel) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.anubisathwarbringer) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.anyfincanhappen) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.aranasibroodmother) bestval += 123;
                                    if (discoverCards[i].card.name == CardDB.cardName.arathiweaponsmith) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneamplifier) bestval += 40;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneanomaly) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneartificer) bestval += 61;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneblast) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanebreath) bestval += 282;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanedevourer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanedynamo) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneexplosion) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneflakmage) bestval += 412;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanefletcher) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanegiant) bestval += 524;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanegolem) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneintellect) bestval += 730;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanekeysmith) bestval += 73;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanemissiles) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanenullifierx21) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneservant) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcaneshot) bestval += 32;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanetyrant) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanewatcher) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanitereaper) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanologist) bestval += 464;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanosaur) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.arcanosmith) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.archbishopbenedictus) bestval += 58;
                                    if (discoverCards[i].card.name == CardDB.cardName.archivistelysiana) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.archmage) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.archmageantonidas) bestval += 73;
                                    if (discoverCards[i].card.name == CardDB.cardName.archmagearugal) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.archmagevargoth) bestval += 739;
                                    if (discoverCards[i].card.name == CardDB.cardName.archthiefrafaam) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.archvillainrafaam) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.arenafanatic) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.arenapatron) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.arenatreasurechest) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.arfus) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.argentcommander) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.argenthorserider) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.argentlance) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.argentprotector) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.argentsquire) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.argentwatchman) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.armagedillo) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.armoredgoon) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.armoredwarhorse) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.armorsmith) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.arrogantcrusader) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.assassinate) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.assassinsblade) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.astralcommunion) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.astralrift) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.astraltiger) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.astromancer) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.auchenaiphantasm) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.auchenaisoulpriest) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.auctionmasterbeardo) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.augmentedelekk) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.autodefensematrix) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.avalanche) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.avenge) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.avengingwrath) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.aviana) bestval += 32;
                                    if (discoverCards[i].card.name == CardDB.cardName.avianwatcher) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.awaken) bestval += 104;
                                    if (discoverCards[i].card.name == CardDB.cardName.awakenthemakers) bestval += 122;
                                    if (discoverCards[i].card.name == CardDB.cardName.axeflinger) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ayablackpaw) bestval += 132;
                                    if (discoverCards[i].card.name == CardDB.cardName.azalinasoulthief) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.azeriteelemental) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.azuredrake) bestval += 202;
                                    if (discoverCards[i].card.name == CardDB.cardName.azureexplorer) bestval += 88;
                                    if (discoverCards[i].card.name == CardDB.cardName.babblingbook) bestval += 107;
                                    if (discoverCards[i].card.name == CardDB.cardName.backstab) bestval += 251;
                                    if (discoverCards[i].card.name == CardDB.cardName.backstreetleper) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.badluckalbatross) bestval += 530;
                                    if (discoverCards[i].card.name == CardDB.cardName.baitedarrow) bestval += 26;
                                    if (discoverCards[i].card.name == CardDB.cardName.bakuthemooneater) bestval += 195;
                                    if (discoverCards[i].card.name == CardDB.cardName.balefulbanker) bestval += 85;
                                    if (discoverCards[i].card.name == CardDB.cardName.ballofspiders) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bananabuffoon) bestval += 291;
                                    if (discoverCards[i].card.name == CardDB.cardName.bandersmosh) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.baneofdoom) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.baristalynchen) bestval += 152;
                                    if (discoverCards[i].card.name == CardDB.cardName.barkskin) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.barnes) bestval += 55;
                                    if (discoverCards[i].card.name == CardDB.cardName.barongeddon) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.baronrivendare) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.barrensstablehand) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bash) bestval += 26;
                                    if (discoverCards[i].card.name == CardDB.cardName.batterhead) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.battlerage) bestval += 48;
                                    if (discoverCards[i].card.name == CardDB.cardName.bazaarburglary) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.bazaarmugger) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.beakeredlightning) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.beamingsidekick) bestval += 33;
                                    if (discoverCards[i].card.name == CardDB.cardName.bearshark) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.beartrap) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.beckonerofevil) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.beeees) bestval += 48;
                                    if (discoverCards[i].card.name == CardDB.cardName.belligerentgnome) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.bellringersentry) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.beneaththegrounds) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.benevolentdjinn) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.berylliumnullifier) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bestialwrath) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.betrayal) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.bewitchedguardian) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bigbadarchmage) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.bigbadvoodoo) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.biggamehunter) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.bigolwhelp) bestval += 242;
                                    if (discoverCards[i].card.name == CardDB.cardName.bigtimeracketeer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bilefintidehunter) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.bindingheal) bestval += 86;
                                    if (discoverCards[i].card.name == CardDB.cardName.biologyproject) bestval += 282;
                                    if (discoverCards[i].card.name == CardDB.cardName.bite) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.biteweed) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bittertidehydra) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.blackcat) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.blackguard) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.blackhowlgunspire) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.blackwaldpixie) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.blackwaterpirate) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.blackwingcorruptor) bestval += 30;
                                    if (discoverCards[i].card.name == CardDB.cardName.blackwingtechnician) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.bladedcultist) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bladedgauntlet) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bladeflurry) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.bladeofcthun) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.blastcrystalpotion) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.blastmasterboom) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.blastwave) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.blatantdecoy) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.blazecaller) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.blazingbattlemage) bestval += 89;
                                    if (discoverCards[i].card.name == CardDB.cardName.blazinginvocation) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.blessedchampion) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.blessingofkings) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.blessingofmight) bestval += 78;
                                    if (discoverCards[i].card.name == CardDB.cardName.blessingoftheancients) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.blessingofwisdom) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.blightnozzlecrawler) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.blingtron3000) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.blinkfox) bestval += 92;
                                    if (discoverCards[i].card.name == CardDB.cardName.blizzard) bestval += 222;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodbloom) bestval += 315;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodclaw) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodfenraptor) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodfurypotion) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodhoofbrave) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodimp) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodknight) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodlust) bestval += 36;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodmagethalnos) bestval += 683;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodoftheancientone) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodqueenlanathel) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodrazor) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodreaverguldan) bestval += 429;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodsailcorsair) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodsailcultist) bestval += 99;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodsailflybooter) bestval += 89;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodsailhowler) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodsailraider) bestval += 43;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodscalpstrategist) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodswornmercenary) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodtoichor) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodtrollsapper) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodwarriors) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodwitch) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bloodworm) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.blowgillsniper) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.blowtorchsaboteur) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.blubberbaron) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bluegillwarrior) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.bodywrapper) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.bogcreeper) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bogshaper) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bogslosher) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.boisterousbard) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bolframshield) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bolster) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.bolvarfordragon) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.bomblobber) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.bombsquad) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bombtoss) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.bombwrangler) bestval += 76;
                                    if (discoverCards[i].card.name == CardDB.cardName.bonebaron) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bonedrake) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.boneguardlieutenant) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.bonemare) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.bonewraith) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.bonfireelemental) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.bookofspecters) bestval += 447;
                                    if (discoverCards[i].card.name == CardDB.cardName.bookwyrm) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.boommasterflark) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.boompistolbully) bestval += 62;
                                    if (discoverCards[i].card.name == CardDB.cardName.boomsquad) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.bootybaybodyguard) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.bootybaybookie) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.boulderfistogre) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bouncingblade) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.brainstormer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.branchingpaths) bestval += 385;
                                    if (discoverCards[i].card.name == CardDB.cardName.brannbronzebeard) bestval += 776;
                                    if (discoverCards[i].card.name == CardDB.cardName.brassknuckles) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bravearcher) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.brawl) bestval += 85;
                                    if (discoverCards[i].card.name == CardDB.cardName.brazenzealot) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.breathofdreams) bestval += 93;
                                    if (discoverCards[i].card.name == CardDB.cardName.breathofsindragosa) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.breathoftheinfinite) bestval += 209;
                                    if (discoverCards[i].card.name == CardDB.cardName.brighteyedscout) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.brightwing) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.bringiton) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.bronzeexplorer) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.bronzegatekeeper) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.bronzeherald) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.brrrloc) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.buccaneer) bestval += 72;
                                    if (discoverCards[i].card.name == CardDB.cardName.bugcollector) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.bulldozer) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.burgle) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.burglybully) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.burlyrockjawtrogg) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.burlyshovelfist) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.cabaliststome) bestval += 41;
                                    if (discoverCards[i].card.name == CardDB.cardName.cabalshadowpriest) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.cairnebloodhoof) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.callinthefinishers) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.callofthevoid) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.callofthewild) bestval += 51;
                                    if (discoverCards[i].card.name == CardDB.cardName.callpet) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.calltoadventure) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.calltoarms) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.camouflageddirigible) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.candlebreath) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.candleshot) bestval += 83;
                                    if (discoverCards[i].card.name == CardDB.cardName.candletaker) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.cannonbarrage) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.captaingreenskin) bestval += 52;
                                    if (discoverCards[i].card.name == CardDB.cardName.captainhooktusk) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.captainsparrot) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.capturedjormungar) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.carnivorouscube) bestval += 293;
                                    if (discoverCards[i].card.name == CardDB.cardName.carriondrake) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.carriongrub) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.cataclysm) bestval += 234;
                                    if (discoverCards[i].card.name == CardDB.cardName.cathedralgargoyle) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.catrinamuerte) bestval += 141;
                                    if (discoverCards[i].card.name == CardDB.cardName.cattrick) bestval += 111;
                                    if (discoverCards[i].card.name == CardDB.cardName.cauldronelemental) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.cavehydra) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.cavernshinyfinder) bestval += 65;
                                    if (discoverCards[i].card.name == CardDB.cardName.celestialdreamer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.celestialemissary) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.cenarius) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.chameleos) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.chaosgazer) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.charge) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.chargeddevilsaur) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.chargedhammer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.cheapshot) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.cheatdeath) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.cheatyanklebiter) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.chefnomi) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.chenvaala) bestval += 54;
                                    if (discoverCards[i].card.name == CardDB.cardName.chiefinspector) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.chillbladechampion) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.chillmaw) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.chillwindyeti) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.chitteringtunneler) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.chogall) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.chopshopcopter) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.chromaggus) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.chromaticegg) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.chronobreaker) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.cinderstorm) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.circleofhealing) bestval += 87;
                                    if (discoverCards[i].card.name == CardDB.cardName.claw) bestval += 38;
                                    if (discoverCards[i].card.name == CardDB.cardName.cleartheway) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.cleave) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.clericofscales) bestval += 55;
                                    if (discoverCards[i].card.name == CardDB.cardName.cleverdisguise) bestval += 72;
                                    if (discoverCards[i].card.name == CardDB.cardName.cloakedhuntress) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.cloakscalechemist) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.clockworkautomaton) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.clockworkgiant) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.clockworkgnome) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.clockworkgoblin) bestval += 34;
                                    if (discoverCards[i].card.name == CardDB.cardName.clockworkknight) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.cloningdevice) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.cloudprince) bestval += 431;
                                    if (discoverCards[i].card.name == CardDB.cardName.clutchmotherzavas) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.cobaltguardian) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.cobaltscalebane) bestval += 33;
                                    if (discoverCards[i].card.name == CardDB.cardName.cobaltspellkin) bestval += 351;
                                    if (discoverCards[i].card.name == CardDB.cardName.cobrashot) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.coffincrasher) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.coghammer) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.cogmaster) bestval += 36;
                                    if (discoverCards[i].card.name == CardDB.cardName.cogmasterswrench) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.coldarradrake) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.coldblood) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.coldlightoracle) bestval += 502;
                                    if (discoverCards[i].card.name == CardDB.cardName.coldlightseer) bestval += 54;
                                    if (discoverCards[i].card.name == CardDB.cardName.coldwraith) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.colossusofthemoon) bestval += 76;
                                    if (discoverCards[i].card.name == CardDB.cardName.commanderrhyssa) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.commandingshout) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.competitivespirit) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.conceal) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.coneofcold) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.confessorpaletress) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.confuse) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.conjuredmirage) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.conjurerscalling) bestval += 67;
                                    if (discoverCards[i].card.name == CardDB.cardName.consecration) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.convert) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.convincinginfiltrator) bestval += 217;
                                    if (discoverCards[i].card.name == CardDB.cardName.coppertailimposter) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.corehound) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.corerager) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.corneredsentry) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.corpseraiser) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.corpsetaker) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.corpsewidow) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.corridorcreeper) bestval += 82;
                                    if (discoverCards[i].card.name == CardDB.cardName.corrosivebreath) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.corrosivesludge) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.corruptedhealbot) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.corruptedseer) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.corruptelementalist) bestval += 48;
                                    if (discoverCards[i].card.name == CardDB.cardName.corruptingmist) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.corruption) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.corruptthewaters) bestval += 148;
                                    if (discoverCards[i].card.name == CardDB.cardName.cosmicanomaly) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.counterfeitcoin) bestval += 65;
                                    if (discoverCards[i].card.name == CardDB.cardName.counterspell) bestval += 504;
                                    if (discoverCards[i].card.name == CardDB.cardName.countessashmore) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.crackle) bestval += 191;
                                    if (discoverCards[i].card.name == CardDB.cardName.cracklingrazormaw) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.crazedalchemist) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.crazedchemist) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.crazednetherwing) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.crazedworshipper) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.crowdfavorite) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.crowdroaster) bestval += 33;
                                    if (discoverCards[i].card.name == CardDB.cardName.crueldinomancer) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.crueltaskmaster) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.crush) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.crushinghand) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.crushingwalls) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.cryomancer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.cryostasis) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.cryptlord) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystallineoracle) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystallion) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystallizer) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystalmerchant) bestval += 41;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystalpower) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystalsmithkangor) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystalsongportal) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystalstag) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystalweaver) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.crystology) bestval += 204;
                                    if (discoverCards[i].card.name == CardDB.cardName.cthun) bestval += 73;
                                    if (discoverCards[i].card.name == CardDB.cardName.cthunschosen) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.cultapothecary) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.cultmaster) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.cultsorcerer) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.cumulomaximus) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.curiocollector) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.curiousglimmerroot) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.cursedblade) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.cursedcastaway) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.curseddisciple) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.curseofrafaam) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.curseofweakness) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.cutpurse) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.cutthroatbuccaneer) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.cybertechchip) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.cyclopianhorror) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.dalaranaspirant) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.dalarancrusader) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dalaranlibrarian) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.dalaranmage) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.damagedstegotron) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.dancingswords) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.daringescape) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.daringfireeater) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.daringreporter) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dariuscrowley) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkarakkoa) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkbargain) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkbomb) bestval += 86;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkconviction) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkcultist) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkesthour) bestval += 93;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkirondwarf) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkironskulker) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkmiremoonkin) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkpact) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkpeddler) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkpharaohtekahn) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkpossession) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkprophecy) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkscalehealer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkshirealchemist) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkshirecouncilman) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkshirelibrarian) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkskies) bestval += 403;
                                    if (discoverCards[i].card.name == CardDB.cardName.darkwispers) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.darnassusaspirant) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.darttrap) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.daundatakah) bestval += 37;
                                    if (discoverCards[i].card.name == CardDB.cardName.deadlyarsenal) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.deadlyfork) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.deadlypoison) bestval += 79;
                                    if (discoverCards[i].card.name == CardDB.cardName.deadlyshot) bestval += 97;
                                    if (discoverCards[i].card.name == CardDB.cardName.deadmanshand) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.deadringer) bestval += 225;
                                    if (discoverCards[i].card.name == CardDB.cardName.deadscaleknight) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathaxepunisher) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathlord) bestval += 176;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathrevenant) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathsbite) bestval += 68;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathspeaker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathstalkerrexxar) bestval += 176;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathwebspider) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathwing) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathwing) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.deathwing) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.deckofwonders) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.defenderofargus) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.defiascleaner) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.defiasringleader) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.defile) bestval += 694;
                                    if (discoverCards[i].card.name == CardDB.cardName.dementedfrostcaller) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.demolisher) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.demonbolt) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.demonfire) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.demonfuse) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.demonheart) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.demonicproject) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.demonwrath) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.dendrologist) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.depthcharge) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.derangeddoctor) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.desertcamel) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.deserthare) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.desertobelisk) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.desertspear) bestval += 59;
                                    if (discoverCards[i].card.name == CardDB.cardName.desperatemeasures) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.desperatestand) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.despicabledreadlord) bestval += 94;
                                    if (discoverCards[i].card.name == CardDB.cardName.devastate) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.devilsauregg) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.devolve) bestval += 346;
                                    if (discoverCards[i].card.name == CardDB.cardName.devotedmaniac) bestval += 317;
                                    if (discoverCards[i].card.name == CardDB.cardName.devourmind) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.dimensionalripper) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.dinomancy) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dinosize) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.dinotamerbrann) bestval += 73;
                                    if (discoverCards[i].card.name == CardDB.cardName.direfrenzy) bestval += 34;
                                    if (discoverCards[i].card.name == CardDB.cardName.direhornhatchling) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.diremole) bestval += 53;
                                    if (discoverCards[i].card.name == CardDB.cardName.direwolfalpha) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.dirtyrat) bestval += 348;
                                    if (discoverCards[i].card.name == CardDB.cardName.discipleofcthun) bestval += 59;
                                    if (discoverCards[i].card.name == CardDB.cardName.discipleofgalakrond) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.diseasedvulture) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.dispatchkodo) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.divinefavor) bestval += 252;
                                    if (discoverCards[i].card.name == CardDB.cardName.divinehymn) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.divinespirit) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.divinestrength) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.divinggryphon) bestval += 66;
                                    if (discoverCards[i].card.name == CardDB.cardName.djinniofzephyrs) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dollmasterdorian) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.donhancho) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.doom) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.doomcaller) bestval += 33;
                                    if (discoverCards[i].card.name == CardDB.cardName.doomedapprentice) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.doomerang) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.doomguard) bestval += 247;
                                    if (discoverCards[i].card.name == CardDB.cardName.doomhammer) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.doomsayer) bestval += 670;
                                    if (discoverCards[i].card.name == CardDB.cardName.doppelgangster) bestval += 122;
                                    if (discoverCards[i].card.name == CardDB.cardName.doublingimp) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.dozingmarksman) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.draeneitotemcarver) bestval += 149;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonbane) bestval += 125;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonblightcultist) bestval += 41;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonbreeder) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragoncalleralanna) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragoncaster) bestval += 84;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonconsort) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonegg) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonfirepotion) bestval += 255;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonhatcher) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonhawkrider) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonkinsorcerer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonlingmechanic) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonmawpoacher) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonmawscorcher) bestval += 38;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonqueenalexstrasza) bestval += 956;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonridertalritha) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonroar) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonsbreath) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonsfury) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonshoard) bestval += 92;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonslayer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonsoul) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonspack) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.dragonspeaker) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.drainlife) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.drainsoul) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.drakkaridefender) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.drakkarienchanter) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.drakkaritrickster) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.drakonidcrusher) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.drakonidoperative) bestval += 109;
                                    if (discoverCards[i].card.name == CardDB.cardName.drboomsscheme) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dreadcorsair) bestval += 123;
                                    if (discoverCards[i].card.name == CardDB.cardName.dreadinfernal) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.dreadraven) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.dreadscale) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dreadsteed) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.dreampetalflorist) bestval += 116;
                                    if (discoverCards[i].card.name == CardDB.cardName.dreamwayguardians) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.drmorrigan) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.druidoftheclaw) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.druidofthefang) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.druidoftheflame) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.druidofthesaber) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.druidofthescythe) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.druidoftheswarm) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.drygulchjailor) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.drywhiskerarmorer) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.duel) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.dunemaulshaman) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.dunesculptor) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.duplicate) bestval += 396;
                                    if (discoverCards[i].card.name == CardDB.cardName.duskbat) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.duskboar) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.duskbreaker) bestval += 116;
                                    if (discoverCards[i].card.name == CardDB.cardName.duskfallenaviana) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.duskhavenhunter) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.dustdevil) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dwarvenarchaeologist) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.dwarvensharpshooter) bestval += 111;
                                    if (discoverCards[i].card.name == CardDB.cardName.dynomatic) bestval += 41;
                                    if (discoverCards[i].card.name == CardDB.cardName.eadricthepure) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.eagerunderling) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.eaglehornbow) bestval += 108;
                                    if (discoverCards[i].card.name == CardDB.cardName.earthelemental) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.earthenmight) bestval += 54;
                                    if (discoverCards[i].card.name == CardDB.cardName.earthenringfarseer) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.earthenscales) bestval += 59;
                                    if (discoverCards[i].card.name == CardDB.cardName.earthquake) bestval += 87;
                                    if (discoverCards[i].card.name == CardDB.cardName.earthshock) bestval += 56;
                                    if (discoverCards[i].card.name == CardDB.cardName.eaterofsecrets) bestval += 286;
                                    if (discoverCards[i].card.name == CardDB.cardName.ebondragonsmith) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.eccentricscribe) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.echoingooze) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.echoofmedivh) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.ectomancy) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.edwinvancleef) bestval += 104;
                                    if (discoverCards[i].card.name == CardDB.cardName.eeriestatue) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.effigy) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.eggnapper) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.elderlongneck) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.eldritchhorror) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.electrastormsurge) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.electrowright) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.elementalallies) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.elementaldestruction) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.elementalevocation) bestval += 270;
                                    if (discoverCards[i].card.name == CardDB.cardName.elementaryreaction) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.elisestarseeker) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.elisetheenlightened) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.elisethetrailblazer) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.elitetaurenchieftain) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.elvenarcher) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.elvenminstrel) bestval += 115;
                                    if (discoverCards[i].card.name == CardDB.cardName.embalmingritual) bestval += 43;
                                    if (discoverCards[i].card.name == CardDB.cardName.emberscaledrake) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.embiggen) bestval += 106;
                                    if (discoverCards[i].card.name == CardDB.cardName.embracedarkness) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.embracetheshadow) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.emeraldexplorer) bestval += 92;
                                    if (discoverCards[i].card.name == CardDB.cardName.emeraldhivequeen) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.emeraldreaver) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.emeriss) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.emperorcobra) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.emperorthaurissan) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.emperorthaurissan) bestval += 999;
                                    if (discoverCards[i].card.name == CardDB.cardName.empoperative) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.enchantedraven) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.enhanceomechano) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.enterthecoliseum) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.entomb) bestval += 103;
                                    if (discoverCards[i].card.name == CardDB.cardName.envenomweapon) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.envoyoflazul) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.equality) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.escapedmanasaber) bestval += 93;
                                    if (discoverCards[i].card.name == CardDB.cardName.eternalsentinel) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.eternalservitude) bestval += 183;
                                    if (discoverCards[i].card.name == CardDB.cardName.eterniumrover) bestval += 62;
                                    if (discoverCards[i].card.name == CardDB.cardName.etherealarcanist) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.etherealconjurer) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.etherealpeddler) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.eureka) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.evasion) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.evasivechimaera) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.evasivedrakonid) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.evasivefeywing) bestval += 127;
                                    if (discoverCards[i].card.name == CardDB.cardName.evasivewyrm) bestval += 134;
                                    if (discoverCards[i].card.name == CardDB.cardName.everyfinisawesome) bestval += 34;
                                    if (discoverCards[i].card.name == CardDB.cardName.evilcablerat) bestval += 85;
                                    if (discoverCards[i].card.name == CardDB.cardName.evilconscripter) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.evilgenius) bestval += 205;
                                    if (discoverCards[i].card.name == CardDB.cardName.evilheckler) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.evilmiscreant) bestval += 187;
                                    if (discoverCards[i].card.name == CardDB.cardName.evilquartermaster) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.evilrecruiter) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.eviltotem) bestval += 211;
                                    if (discoverCards[i].card.name == CardDB.cardName.eviscerate) bestval += 198;
                                    if (discoverCards[i].card.name == CardDB.cardName.evolve) bestval += 33;
                                    if (discoverCards[i].card.name == CardDB.cardName.evolvedkobold) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.evolvingspores) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.excavatedevil) bestval += 87;
                                    if (discoverCards[i].card.name == CardDB.cardName.execute) bestval += 32;
                                    if (discoverCards[i].card.name == CardDB.cardName.exoticmountseller) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.expiredmerchant) bestval += 143;
                                    if (discoverCards[i].card.name == CardDB.cardName.explodinator) bestval += 53;
                                    if (discoverCards[i].card.name == CardDB.cardName.explodingbloatbat) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.explorershat) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.exploreungoro) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.explosiveevolution) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.explosiverunes) bestval += 443;
                                    if (discoverCards[i].card.name == CardDB.cardName.explosivesheep) bestval += 38;
                                    if (discoverCards[i].card.name == CardDB.cardName.explosiveshot) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.explosivetrap) bestval += 171;
                                    if (discoverCards[i].card.name == CardDB.cardName.extraarms) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.eydisdarkbane) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.eyeforaneye) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.eyeofthestorm) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.facecollector) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.facelessbehemoth) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.facelesscorruptor) bestval += 238;
                                    if (discoverCards[i].card.name == CardDB.cardName.facelesslurker) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.facelessmanipulator) bestval += 229;
                                    if (discoverCards[i].card.name == CardDB.cardName.facelessrager) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.facelessshambler) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.facelesssummoner) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.faeriedragon) bestval += 138;
                                    if (discoverCards[i].card.name == CardDB.cardName.faithfullumi) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.faldoreistrider) bestval += 34;
                                    if (discoverCards[i].card.name == CardDB.cardName.fallenhero) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.fallensuncleric) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.fandralstaghelm) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.fanofknives) bestval += 126;
                                    if (discoverCards[i].card.name == CardDB.cardName.farrakibattleaxe) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.farsight) bestval += 133;
                                    if (discoverCards[i].card.name == CardDB.cardName.fatespinner) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.fateweaver) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.fearsomedoomguard) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.feedingtime) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.feigndeath) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.felcannon) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.felfirepotion) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.felguard) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.fellordbetrug) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.felorcsoulfiend) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.felreaver) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.felsoulinquisitor) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.felstalker) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.fencingcoach) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.fencreeper) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.feralgibberer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.feralrage) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.feralspirit) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.ferocioushowl) bestval += 414;
                                    if (discoverCards[i].card.name == CardDB.cardName.festeroothulk) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.feugen) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.fiendishcircle) bestval += 96;
                                    if (discoverCards[i].card.name == CardDB.cardName.fiendishrites) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.fiendishservant) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.fiercemonkey) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.fierybat) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.fierywaraxe) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.fightpromoter) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.finderskeepers) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.fireball) bestval += 483;
                                    if (discoverCards[i].card.name == CardDB.cardName.fireelemental) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.firefly) bestval += 103;
                                    if (discoverCards[i].card.name == CardDB.cardName.fireguarddestroyer) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.firehawk) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.firelandsportal) bestval += 48;
                                    if (discoverCards[i].card.name == CardDB.cardName.fireplumeharbinger) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.fireplumephoenix) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.fireplumesheart) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.firetreewitchdoctor) bestval += 177;
                                    if (discoverCards[i].card.name == CardDB.cardName.fireworkstech) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.fishflinger) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.fistofjaraxxus) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.fjolalightbane) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.flamecannon) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.flamegeyser) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.flameimp) bestval += 50;
                                    if (discoverCards[i].card.name == CardDB.cardName.flamejuggler) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.flamelance) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.flameleviathan) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.flamestrike) bestval += 138;
                                    if (discoverCards[i].card.name == CardDB.cardName.flametonguetotem) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.flamewaker) bestval += 267;
                                    if (discoverCards[i].card.name == CardDB.cardName.flameward) bestval += 711;
                                    if (discoverCards[i].card.name == CardDB.cardName.flamewreathedfaceless) bestval += 125;
                                    if (discoverCards[i].card.name == CardDB.cardName.flankingstrike) bestval += 84;
                                    if (discoverCards[i].card.name == CardDB.cardName.flare) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.flarksboomzooka) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.flashheal) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.flashoflight) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.flesheatingghoul) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.flightmaster) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.flikskyshiv) bestval += 142;
                                    if (discoverCards[i].card.name == CardDB.cardName.floatingwatcher) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.flobbidinousfloop) bestval += 83;
                                    if (discoverCards[i].card.name == CardDB.cardName.floopsgloriousgloop) bestval += 142;
                                    if (discoverCards[i].card.name == CardDB.cardName.flyingmachine) bestval += 147;
                                    if (discoverCards[i].card.name == CardDB.cardName.foereaper4000) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.foolsbane) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.forbiddenancient) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.forbiddenflame) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.forbiddenhealing) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.forbiddenritual) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.forbiddenshaping) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.forbiddenwords) bestval += 334;
                                    if (discoverCards[i].card.name == CardDB.cardName.forceofnature) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.forcetankmax) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.forestguide) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.forgeofsouls) bestval += 34;
                                    if (discoverCards[i].card.name == CardDB.cardName.forgottentorch) bestval += 128;
                                    if (discoverCards[i].card.name == CardDB.cardName.forkedlightning) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.forlornstalker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.formerchamp) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.fossilizeddevilsaur) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.freefromamber) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.freezingpotion) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.freezingtrap) bestval += 147;
                                    if (discoverCards[i].card.name == CardDB.cardName.frenziedfelwing) bestval += 133;
                                    if (discoverCards[i].card.name == CardDB.cardName.freshscent) bestval += 55;
                                    if (discoverCards[i].card.name == CardDB.cardName.friendlybartender) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.frightenedflunky) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.frigidsnobold) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.frizzkindleroost) bestval += 115;
                                    if (discoverCards[i].card.name == CardDB.cardName.frostbolt) bestval += 199;
                                    if (discoverCards[i].card.name == CardDB.cardName.frostelemental) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.frostgiant) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.frostlichjaina) bestval += 141;
                                    if (discoverCards[i].card.name == CardDB.cardName.frostnova) bestval += 274;
                                    if (discoverCards[i].card.name == CardDB.cardName.frostshock) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.frostwolfgrunt) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.frostwolfwarlord) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.frothingberserker) bestval += 56;
                                    if (discoverCards[i].card.name == CardDB.cardName.frozenclone) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.frozencrusher) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.fungalenchanter) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.fungalmancer) bestval += 118;
                                    if (discoverCards[i].card.name == CardDB.cardName.furbolgmossbinder) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.furiousettin) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.furnacefirecolossus) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.gadgetzanauctioneer) bestval += 90;
                                    if (discoverCards[i].card.name == CardDB.cardName.gadgetzanferryman) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.gadgetzanjouster) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.gadgetzansocialite) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.gahzrilla) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.galvanizer) bestval += 234;
                                    if (discoverCards[i].card.name == CardDB.cardName.gangup) bestval += 49;
                                    if (discoverCards[i].card.name == CardDB.cardName.gardengnome) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.garrisoncommander) bestval += 34;
                                    if (discoverCards[i].card.name == CardDB.cardName.gatheryourparty) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.gazlowe) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.gelbinmekkatorque) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.gemstuddedgolem) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.generousmummy) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.genngreymane) bestval += 264;
                                    if (discoverCards[i].card.name == CardDB.cardName.gentlemegasaur) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.geosculptoryip) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.getawaykodo) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.ghastlyconjurer) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.ghostlightangler) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.ghostlycharger) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.giantanaconda) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.giantmastodon) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.giantsandworm) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.giantwasp) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.giftofthewild) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.gigglinginventor) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.gilblinstalker) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.gildedgargoyle) bestval += 30;
                                    if (discoverCards[i].card.name == CardDB.cardName.gilneanroyalguard) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.glacialmysteries) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.glacialshard) bestval += 107;
                                    if (discoverCards[i].card.name == CardDB.cardName.gladiatorslongbow) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.glaivezooka) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.glindacrowskin) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.glittermoth) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.gloomstag) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.gloopsprayer) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.glowstonetechnician) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.glowtron) bestval += 175;
                                    if (discoverCards[i].card.name == CardDB.cardName.gluttonousooze) bestval += 149;
                                    if (discoverCards[i].card.name == CardDB.cardName.gnash) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.gnomeferatu) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.gnomereganinfantry) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.gnomishexperimenter) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.gnomishinventor) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.goblinautobarber) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.goblinblastmage) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.goblinbomb) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.goblinprank) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.goblinsapper) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.goboglidetech) bestval += 26;
                                    if (discoverCards[i].card.name == CardDB.cardName.golakkacrawler) bestval += 52;
                                    if (discoverCards[i].card.name == CardDB.cardName.goldenscarab) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.goldshirefootman) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.gorehowl) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.gorillabota3) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.gormoktheimpaler) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.goruthemightree) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.grandarchivist) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.grandcrusader) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.grandlackeyerkh) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.grandmummy) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.gravehorror) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.gravelsnoutknight) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.graverune) bestval += 104;
                                    if (discoverCards[i].card.name == CardDB.cardName.graveshambler) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.greaterarcanemissiles) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.greaterhealingpotion) bestval += 150;
                                    if (discoverCards[i].card.name == CardDB.cardName.greedysprite) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.greenjelly) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.grievousbite) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.griftah) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimestreetenforcer) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimestreetinformant) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimestreetoutfitter) bestval += 152;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimestreetpawnbroker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimestreetprotector) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimestreetsmuggler) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimnecromancer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimpatron) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimrally) bestval += 37;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimscalechum) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimscaleoracle) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.grimygadgeteer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.grizzledguardian) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.grizzledwizard) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.grommashhellscream) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.grookfumaster) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.grotesquedragonhawk) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.grovetender) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.gruul) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.guardianofkings) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.guildrecruiter) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.gurubashiberserker) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.gurubashichicken) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.gurubashihypemon) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.gurubashioffering) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.gyrocopter) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.hackthesystem) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.hadronox) bestval += 37;
                                    if (discoverCards[i].card.name == CardDB.cardName.hagathasscheme) bestval += 111;
                                    if (discoverCards[i].card.name == CardDB.cardName.hagathathewitch) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.hailbringer) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.halftimescavenger) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.hallazealtheascended) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.hallucination) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.hammeroftwilight) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.hammerofwrath) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.handofprotection) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.happyghoul) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.harbingercelestia) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.harrisonjones) bestval += 32;
                                    if (discoverCards[i].card.name == CardDB.cardName.harvestgolem) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.hauntedcreeper) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.hauntingvisions) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.headcrack) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.headhuntershatchet) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.healingrain) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.healingtouch) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.healingwave) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.heavymetal) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.hecklebot) bestval += 50;
                                    if (discoverCards[i].card.name == CardDB.cardName.heistbarontogwaggle) bestval += 125;
                                    if (discoverCards[i].card.name == CardDB.cardName.hellfire) bestval += 153;
                                    if (discoverCards[i].card.name == CardDB.cardName.helplesshatchling) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.hemetnesingwary) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.henchclanburglar) bestval += 89;
                                    if (discoverCards[i].card.name == CardDB.cardName.henchclanhag) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.henchclanhogsteed) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.henchclanshadequill) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.henchclansneak) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.henchclanthug) bestval += 57;
                                    if (discoverCards[i].card.name == CardDB.cardName.heraldvolazj) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.heroicinnkeeper) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.heroicstrike) bestval += 71;
                                    if (discoverCards[i].card.name == CardDB.cardName.hex) bestval += 176;
                                    if (discoverCards[i].card.name == CardDB.cardName.hexlordmalacrass) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.hiddencache) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.hiddenoasis) bestval += 57;
                                    if (discoverCards[i].card.name == CardDB.cardName.hiddenwisdom) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.highinquisitorwhitemane) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.highpriestamet) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.highpriestessjeklik) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.highpriestthekal) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.hippogryph) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.hiredgun) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.historybuff) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.hoardingdragon) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.hoardpillager) bestval += 38;
                                    if (discoverCards[i].card.name == CardDB.cardName.hobartgrapplehammer) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.hobgoblin) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.hogger) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.holomancer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.holychampion) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.holyfire) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.holylight) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.holynova) bestval += 50;
                                    if (discoverCards[i].card.name == CardDB.cardName.holyripple) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.holysmite) bestval += 181;
                                    if (discoverCards[i].card.name == CardDB.cardName.holywater) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.holywrath) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.hoodedacolyte) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.hookedreaver) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.hookedscimitar) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.hotairballoon) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.hotspringguardian) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.houndmaster) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.houndmastershaw) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.howlfiend) bestval += 26;
                                    if (discoverCards[i].card.name == CardDB.cardName.howlingcommander) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.hozenhealer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.hugetoad) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.humility) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.humongousrazorleaf) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.hungrycrab) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.hungrydragon) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.hungryettin) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.huntersmark) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.hunterspack) bestval += 33;
                                    if (discoverCards[i].card.name == CardDB.cardName.huntingmastiff) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.huntingparty) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.hydrologist) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.hyenaalpha) bestval += 76;
                                    if (discoverCards[i].card.name == CardDB.cardName.hyldnirfrostrider) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.icebarrier) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.iceblock) bestval += 815;
                                    if (discoverCards[i].card.name == CardDB.cardName.icebreaker) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.icecreampeddler) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.icefishing) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.icehowl) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.icelance) bestval += 30;
                                    if (discoverCards[i].card.name == CardDB.cardName.icerager) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.icewalker) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.icicle) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.igneouselemental) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.iknowaguy) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.illidanstormrage) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.illuminator) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.immortalprelate) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.impbalming) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.impferno) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.impgangboss) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.implosion) bestval += 74;
                                    if (discoverCards[i].card.name == CardDB.cardName.impmaster) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.improvemorale) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.infest) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.infestedgoblin) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.infestedtauren) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.infestedwolf) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.injuredblademaster) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.injuredkvaldir) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.injuredtolvir) bestval += 50;
                                    if (discoverCards[i].card.name == CardDB.cardName.inkmastersolia) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.innerfire) bestval += 36;
                                    if (discoverCards[i].card.name == CardDB.cardName.innerrage) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.innervate) bestval += 298;
                                    if (discoverCards[i].card.name == CardDB.cardName.intothefray) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.invocationoffrost) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironbarkprotector) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironbeakowl) bestval += 143;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironforgeportal) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironforgerifleman) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironfurgrizzly) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironhide) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironhidedirehorn) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironjuggernaut) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironsensei) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ironwoodgolem) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.ivoryknight) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadebehemoth) bestval += 60;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadeblossom) bestval += 224;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadechieftain) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadeclaws) bestval += 70;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadeidol) bestval += 224;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadelightning) bestval += 71;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadeshuriken) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadespirit) bestval += 75;
                                    if (discoverCards[i].card.name == CardDB.cardName.jadeswarmer) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.jardealer) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.jeeves) bestval += 114;
                                    if (discoverCards[i].card.name == CardDB.cardName.jepettojoybuzz) bestval += 115;
                                    if (discoverCards[i].card.name == CardDB.cardName.jeweledmacaw) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.jeweledscarab) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.jinyuwaterspeaker) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.journeybelow) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.juicypsychmelon) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.jumboimp) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.junglegiants) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.junglemoonkin) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.junglepanther) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.junkbot) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.justicartrueheart) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.kabalchemist) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.kabalcourier) bestval += 36;
                                    if (discoverCards[i].card.name == CardDB.cardName.kabalcrystalrunner) bestval += 424;
                                    if (discoverCards[i].card.name == CardDB.cardName.kaballackey) bestval += 416;
                                    if (discoverCards[i].card.name == CardDB.cardName.kabalsongstealer) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.kabaltalonpriest) bestval += 37;
                                    if (discoverCards[i].card.name == CardDB.cardName.kabaltrafficker) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.kaboombot) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.kaelthassunstrider) bestval += 233;
                                    if (discoverCards[i].card.name == CardDB.cardName.kalecgos) bestval += 121;
                                    if (discoverCards[i].card.name == CardDB.cardName.kangorsendlessarmy) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.karakazham) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.kathrenawinterwisp) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.kazakus) bestval += 908;
                                    if (discoverCards[i].card.name == CardDB.cardName.keeningbanshee) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.keeperofthegrove) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.keeperofuldaman) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.keeperstalladris) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.kelthuzad) bestval += 145;
                                    if (discoverCards[i].card.name == CardDB.cardName.kezanmystic) bestval += 133;
                                    if (discoverCards[i].card.name == CardDB.cardName.khadgar) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.khartutdefender) bestval += 427;
                                    if (discoverCards[i].card.name == CardDB.cardName.kidnapper) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.killcommand) bestval += 123;
                                    if (discoverCards[i].card.name == CardDB.cardName.kindlygrandmother) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingkrush) bestval += 26;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingmosh) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingmukla) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingofbeasts) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingphaoris) bestval += 57;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingsbane) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingsdefender) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingselekk) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.kingtogwaggle) bestval += 75;
                                    if (discoverCards[i].card.name == CardDB.cardName.kirintormage) bestval += 435;
                                    if (discoverCards[i].card.name == CardDB.cardName.kirintortricaster) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.klaxxiamberweaver) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.knifejuggler) bestval += 53;
                                    if (discoverCards[i].card.name == CardDB.cardName.knightofthewild) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.knuckles) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldapprentice) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldbarbarian) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldgeomancer) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldhermit) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldillusionist) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldlibrarian) bestval += 552;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldmonk) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldsandtrooper) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.koboldstickyfinger) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.kodorider) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.kookychemist) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.korkronelite) bestval += 159;
                                    if (discoverCards[i].card.name == CardDB.cardName.kronxdragonhoof) bestval += 287;
                                    if (discoverCards[i].card.name == CardDB.cardName.krultheunshackled) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.kuntheforgottenking) bestval += 53;
                                    if (discoverCards[i].card.name == CardDB.cardName.kvaldirraider) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.labrecruiter) bestval += 106;
                                    if (discoverCards[i].card.name == CardDB.cardName.ladyinwhite) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.lakkarifelhound) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.lakkarisacrifice) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.lancecarrier) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.landscaping) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.lavaburst) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.lavashock) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.layonhands) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.lazulsscheme) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.learndraconic) bestval += 34;
                                    if (discoverCards[i].card.name == CardDB.cardName.leathercladhogleader) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.leechingpoison) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.leeroyjenkins) bestval += 518;
                                    if (discoverCards[i].card.name == CardDB.cardName.lepergnome) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.lesseramethystspellstone) bestval += 88;
                                    if (discoverCards[i].card.name == CardDB.cardName.lesserdiamondspellstone) bestval += 160;
                                    if (discoverCards[i].card.name == CardDB.cardName.lesseremeraldspellstone) bestval += 68;
                                    if (discoverCards[i].card.name == CardDB.cardName.lesserjasperspellstone) bestval += 338;
                                    if (discoverCards[i].card.name == CardDB.cardName.lessermithrilspellstone) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.lesseronyxspellstone) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.lesserpearlspellstone) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.lesserrubyspellstone) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.lessersapphirespellstone) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.levelup) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.leylinemanipulator) bestval += 38;
                                    if (discoverCards[i].card.name == CardDB.cardName.licensedadventurer) bestval += 503;
                                    if (discoverCards[i].card.name == CardDB.cardName.lifedrinker) bestval += 198;
                                    if (discoverCards[i].card.name == CardDB.cardName.lifeweaver) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightbomb) bestval += 235;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightforgedblessing) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightforgedcrusader) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightforgedzealot) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightfusedstegodon) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightningbolt) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightningbreath) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightningstorm) bestval += 77;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightofthenaaru) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightschampion) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightsjustice) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightspawn) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightssorrow) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightwarden) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.lightwell) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.likkim) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.lilexorcist) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.lilianvoss) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.linecracker) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.livewirelance) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.livingdragonbreath) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.livingmana) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.livingmonument) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.livingroots) bestval += 183;
                                    if (discoverCards[i].card.name == CardDB.cardName.loatheb) bestval += 480;
                                    if (discoverCards[i].card.name == CardDB.cardName.lockandload) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.lonechampion) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.loosespecimen) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.loothoarder) bestval += 530;
                                    if (discoverCards[i].card.name == CardDB.cardName.lordgodfrey) bestval += 198;
                                    if (discoverCards[i].card.name == CardDB.cardName.lordjaraxxus) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.lordofthearena) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.lorewalkercho) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.lostinthejungle) bestval += 92;
                                    if (discoverCards[i].card.name == CardDB.cardName.lostspirit) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.lotusagents) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.lotusassassin) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.lotusillusionist) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.lowlysquire) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.lucentbark) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.luckydobuccaneer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.lunarvisions) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.lunaspocketgalaxy) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.lynessasunsorrow) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.lyrathesunshard) bestval += 68;
                                    if (discoverCards[i].card.name == CardDB.cardName.madamelazul) bestval += 64;
                                    if (discoverCards[i].card.name == CardDB.cardName.madamgoya) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.madbomber) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.madderbomber) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.madhatter) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.madscientist) bestval += 810;
                                    if (discoverCards[i].card.name == CardDB.cardName.madsummoner) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.maelstromportal) bestval += 214;
                                    if (discoverCards[i].card.name == CardDB.cardName.maexxna) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.magiccarpet) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.magicdartfrog) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.magictrick) bestval += 376;
                                    if (discoverCards[i].card.name == CardDB.cardName.magmarager) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.maidenofthelake) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.majordomoexecutus) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.makingmummies) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.malchezaarsimp) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.malfurionthepestilent) bestval += 207;
                                    if (discoverCards[i].card.name == CardDB.cardName.malganis) bestval += 405;
                                    if (discoverCards[i].card.name == CardDB.cardName.malkorok) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.malorne) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.malygos) bestval += 156;
                                    if (discoverCards[i].card.name == CardDB.cardName.manaaddict) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.manabind) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.manacyclone) bestval += 344;
                                    if (discoverCards[i].card.name == CardDB.cardName.manageode) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.managiant) bestval += 494;
                                    if (discoverCards[i].card.name == CardDB.cardName.manareservoir) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.manatidetotem) bestval += 85;
                                    if (discoverCards[i].card.name == CardDB.cardName.manawraith) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.manawyrm) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.manicsoulcaster) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.marinthefox) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.markedshot) bestval += 54;
                                    if (discoverCards[i].card.name == CardDB.cardName.markofnature) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.markoftheloa) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.markofthelotus) bestval += 50;
                                    if (discoverCards[i].card.name == CardDB.cardName.markofthewild) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.markofyshaarj) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.marshdrake) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.maskedcontender) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.massdispel) bestval += 388;
                                    if (discoverCards[i].card.name == CardDB.cardName.masshysteria) bestval += 634;
                                    if (discoverCards[i].card.name == CardDB.cardName.massresurrection) bestval += 170;
                                    if (discoverCards[i].card.name == CardDB.cardName.masterjouster) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.masteroakheart) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.masterofceremonies) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.masterofdisguise) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.masterofevolution) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.masterscall) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.masterswordsmith) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.mayornoggenfogger) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.meanstreetmarshal) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.meatwagon) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.mechanicalwhelp) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.mechanicalyeti) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.mechanoegg) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.mecharoo) bestval += 79;
                                    if (discoverCards[i].card.name == CardDB.cardName.mechathun) bestval += 270;
                                    if (discoverCards[i].card.name == CardDB.cardName.mechbearcat) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.mechwarper) bestval += 199;
                                    if (discoverCards[i].card.name == CardDB.cardName.medivhsvalet) bestval += 435;
                                    if (discoverCards[i].card.name == CardDB.cardName.mekgineerthermaplugg) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.menacingnimbus) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.menageriemagician) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.menageriewarden) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.messengerraven) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.metaltoothleaper) bestval += 40;
                                    if (discoverCards[i].card.name == CardDB.cardName.meteor) bestval += 49;
                                    if (discoverCards[i].card.name == CardDB.cardName.meteorologist) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.micromachine) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.micromummy) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.microtechcontroller) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.midnightdrake) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.militiacommander) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.millhousemanastorm) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.mimicpod) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.mimironshead) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.mindblast) bestval += 326;
                                    if (discoverCards[i].card.name == CardDB.cardName.mindbreaker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.mindcontrol) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.mindcontroltech) bestval += 121;
                                    if (discoverCards[i].card.name == CardDB.cardName.mindflayerkaahrj) bestval += 48;
                                    if (discoverCards[i].card.name == CardDB.cardName.mindgames) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.mindvision) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.minimage) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.miragecaller) bestval += 86;
                                    if (discoverCards[i].card.name == CardDB.cardName.mirekeeper) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.mirrorentity) bestval += 73;
                                    if (discoverCards[i].card.name == CardDB.cardName.mirrorimage) bestval += 86;
                                    if (discoverCards[i].card.name == CardDB.cardName.mischiefmaker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.misdirection) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.missilelauncher) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.mistressofmixtures) bestval += 518;
                                    if (discoverCards[i].card.name == CardDB.cardName.moatlurker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.mogorschampion) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.mogortheogre) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.mogucultist) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.mogufleshshaper) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.mogushanwarden) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.mojomasterzihi) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.moltenblade) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.moltenbreath) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.moltengiant) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.moltenreflection) bestval += 62;
                                    if (discoverCards[i].card.name == CardDB.cardName.moonfire) bestval += 180;
                                    if (discoverCards[i].card.name == CardDB.cardName.moongladeportal) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.moorabi) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.moroes) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.mortalcoil) bestval += 277;
                                    if (discoverCards[i].card.name == CardDB.cardName.mortalstrike) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.mortuarymachine) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.moshoggannouncer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.moshoggenforcer) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.mossyhorror) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.mountainfirearmor) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.mountaingiant) bestval += 107;
                                    if (discoverCards[i].card.name == CardDB.cardName.mountedraptor) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.muckhunter) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.muckmorpher) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.muklaschampion) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.mulch) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.mulchmuncher) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.multishot) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.murksparkeel) bestval += 181;
                                    if (discoverCards[i].card.name == CardDB.cardName.murlocknight) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.murlocraider) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.murloctastyfin) bestval += 53;
                                    if (discoverCards[i].card.name == CardDB.cardName.murloctidecaller) bestval += 59;
                                    if (discoverCards[i].card.name == CardDB.cardName.murloctidehunter) bestval += 32;
                                    if (discoverCards[i].card.name == CardDB.cardName.murloctinyfin) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.murlocwarleader) bestval += 69;
                                    if (discoverCards[i].card.name == CardDB.cardName.murmuringelemental) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.murmy) bestval += 49;
                                    if (discoverCards[i].card.name == CardDB.cardName.murozondtheinfinite) bestval += 57;
                                    if (discoverCards[i].card.name == CardDB.cardName.museumcurator) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.musterforbattle) bestval += 104;
                                    if (discoverCards[i].card.name == CardDB.cardName.mutate) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.myrarotspring) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.myrasunstableelement) bestval += 36;
                                    if (discoverCards[i].card.name == CardDB.cardName.mysteriousblade) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.mysteriouschallenger) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.nagacorsair) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.nagasandwitch) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.nagaseawitch) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.natpagle) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.naturalize) bestval += 263;
                                    if (discoverCards[i].card.name == CardDB.cardName.necriumapothecary) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.necriumblade) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.necriumvial) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.necromechanic) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.necroticgeist) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.nefarian) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.nefersetritualist) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.nefersetthrasher) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.neptulon) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.nerubarweblord) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.nerubianegg) bestval += 179;
                                    if (discoverCards[i].card.name == CardDB.cardName.nerubianprophet) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.nerubianunraveler) bestval += 93;
                                    if (discoverCards[i].card.name == CardDB.cardName.nestingroc) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.netherbreath) bestval += 38;
                                    if (discoverCards[i].card.name == CardDB.cardName.nethersoulbuster) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.netherspitehistorian) bestval += 122;
                                    if (discoverCards[i].card.name == CardDB.cardName.neversurrender) bestval += 69;
                                    if (discoverCards[i].card.name == CardDB.cardName.nexuschampionsaraad) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.nightbanetemplar) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.nightblade) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.nighthowler) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.nightmareamalgam) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.nightprowler) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.nightscalematriarch) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.ninelives) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.nithogg) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.noblesacrifice) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.northseakraken) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.northshirecleric) bestval += 445;
                                    if (discoverCards[i].card.name == CardDB.cardName.nourish) bestval += 287;
                                    if (discoverCards[i].card.name == CardDB.cardName.noviceengineer) bestval += 587;
                                    if (discoverCards[i].card.name == CardDB.cardName.nozari) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.nozdormu) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.nozdormuthetimeless) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.nzothsfirstmate) bestval += 151;
                                    if (discoverCards[i].card.name == CardDB.cardName.oakensummons) bestval += 184;
                                    if (discoverCards[i].card.name == CardDB.cardName.oasissnapjaw) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.oasissurger) bestval += 104;
                                    if (discoverCards[i].card.name == CardDB.cardName.oblivitron) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.obsidiandestroyer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.obsidianshard) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.obsidianstatue) bestval += 195;
                                    if (discoverCards[i].card.name == CardDB.cardName.octosari) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.ogrebrute) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ogremagi) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ogrewarmaul) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.oldmurkeye) bestval += 43;
                                    if (discoverCards[i].card.name == CardDB.cardName.omegaagent) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.omegaassembly) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.omegadefender) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.omegadevastator) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.omegamedic) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.omegamind) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.oneeyedcheat) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.onthehunt) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.onyxbishop) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.onyxia) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.oondasta) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.openthewaygate) bestval += 567;
                                    if (discoverCards[i].card.name == CardDB.cardName.orgrimmaraspirant) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ornerydirehorn) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.ornerytortoise) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.overflow) bestval += 167;
                                    if (discoverCards[i].card.name == CardDB.cardName.overlordswhip) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.ozruk) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.pantryspider) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.parachutebrigand) bestval += 175;
                                    if (discoverCards[i].card.name == CardDB.cardName.paragonoflight) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.patchesthepirate) bestval += 304;
                                    if (discoverCards[i].card.name == CardDB.cardName.patientassassin) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.penance) bestval += 413;
                                    if (discoverCards[i].card.name == CardDB.cardName.perditionsblade) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.phalanxcommander) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.phantomfreebooter) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.phantommilitia) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.pharaohcat) bestval += 123;
                                    if (discoverCards[i].card.name == CardDB.cardName.pharaohsblessing) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.phasestalker) bestval += 151;
                                    if (discoverCards[i].card.name == CardDB.cardName.pickpocket) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.pilfer) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.pilferedpower) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.pilotedreaper) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.pilotedshredder) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.pilotedskygolem) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.pintsizedsummoner) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.pintsizepotion) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.piranhalauncher) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.pitcrocolisk) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.pitfighter) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.pitlord) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.pitsnake) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.plaguebringer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.plagueofdeath) bestval += 195;
                                    if (discoverCards[i].card.name == CardDB.cardName.plagueofflames) bestval += 560;
                                    if (discoverCards[i].card.name == CardDB.cardName.plagueofmadness) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.plagueofmurlocs) bestval += 89;
                                    if (discoverCards[i].card.name == CardDB.cardName.plagueofwrath) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.plaguescientist) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.platebreaker) bestval += 26;
                                    if (discoverCards[i].card.name == CardDB.cardName.platedbeetle) bestval += 62;
                                    if (discoverCards[i].card.name == CardDB.cardName.playdead) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.plottwist) bestval += 140;
                                    if (discoverCards[i].card.name == CardDB.cardName.pogohopper) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.poisonseeds) bestval += 228;
                                    if (discoverCards[i].card.name == CardDB.cardName.pollutedhoarder) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.polymorph) bestval += 328;
                                    if (discoverCards[i].card.name == CardDB.cardName.polymorphboar) bestval += 45;
                                    if (discoverCards[i].card.name == CardDB.cardName.pompousthespian) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.portalkeeper) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.portaloverfiend) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.possessedlackey) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.possessedvillager) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.potionofheroism) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.potionofmadness) bestval += 255;
                                    if (discoverCards[i].card.name == CardDB.cardName.potionofpolymorph) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.potionvendor) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.pounce) bestval += 164;
                                    if (discoverCards[i].card.name == CardDB.cardName.powermace) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.powerofcreation) bestval += 57;
                                    if (discoverCards[i].card.name == CardDB.cardName.powerofthewild) bestval += 70;
                                    if (discoverCards[i].card.name == CardDB.cardName.poweroverwhelming) bestval += 177;
                                    if (discoverCards[i].card.name == CardDB.cardName.powershot) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.powerwordglory) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.powerwordreplicate) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.powerwordshield) bestval += 498;
                                    if (discoverCards[i].card.name == CardDB.cardName.powerwordtentacles) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.praisegalakrond) bestval += 142;
                                    if (discoverCards[i].card.name == CardDB.cardName.predatoryinstincts) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.preparation) bestval += 148;
                                    if (discoverCards[i].card.name == CardDB.cardName.pressureplate) bestval += 57;
                                    if (discoverCards[i].card.name == CardDB.cardName.priestessofelune) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.priestofthefeast) bestval += 91;
                                    if (discoverCards[i].card.name == CardDB.cardName.primalfinchampion) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.primalfinlookout) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.primalfintotem) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.primalfusion) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.primaltalismans) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.primordialdrake) bestval += 66;
                                    if (discoverCards[i].card.name == CardDB.cardName.primordialexplorer) bestval += 103;
                                    if (discoverCards[i].card.name == CardDB.cardName.primordialglyph) bestval += 625;
                                    if (discoverCards[i].card.name == CardDB.cardName.princekeleseth) bestval += 49;
                                    if (discoverCards[i].card.name == CardDB.cardName.princeliam) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.princemalchezaar) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.princesshuhuran) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.princesstalanji) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.princetaldaram) bestval += 59;
                                    if (discoverCards[i].card.name == CardDB.cardName.princevalanar) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.prismaticlens) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.professorputricide) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.prophetvelen) bestval += 251;
                                    if (discoverCards[i].card.name == CardDB.cardName.protecttheking) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.prouddefender) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.psionicprobe) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.psychicscream) bestval += 598;
                                    if (discoverCards[i].card.name == CardDB.cardName.psychopomp) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.psychotron) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.pterrordaxhatchling) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.publicdefender) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.puddlestomper) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.pumpkinpeasant) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.purify) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.puzzleboxofyoggsaron) bestval += 64;
                                    if (discoverCards[i].card.name == CardDB.cardName.pyroblast) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.pyromaniac) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.pyros) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.quartermaster) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.quartzelemental) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.queenofpain) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.questingadventurer) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.questingexplorer) bestval += 694;
                                    if (discoverCards[i].card.name == CardDB.cardName.quicksandelemental) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.quickshot) bestval += 87;
                                    if (discoverCards[i].card.name == CardDB.cardName.rabblebouncer) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.rabidworgen) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.radiance) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.radiantelemental) bestval += 147;
                                    if (discoverCards[i].card.name == CardDB.cardName.rafaamsscheme) bestval += 97;
                                    if (discoverCards[i].card.name == CardDB.cardName.ragingworgen) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ragnarosthefirelord) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ragnarosthefirelord) bestval += 200;
                                    if (discoverCards[i].card.name == CardDB.cardName.raidingparty) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.raidleader) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.raidtheskytemple) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.rainoffire) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.rainoftoads) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.rallyingblade) bestval += 38;
                                    if (discoverCards[i].card.name == CardDB.cardName.ramkahenwildtamer) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.rammingspeed) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.rampage) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.ramwrangler) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.rapidfire) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.raptorhatchling) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.ratcatcher) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.ratpack) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.rattlingrascal) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.rattrap) bestval += 98;
                                    if (discoverCards[i].card.name == CardDB.cardName.ravagingghoul) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.ravasaurrunt) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ravencaller) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.ravenfamiliar) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.ravenholdtassassin) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ravenidol) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.ravenouspterrordax) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.rayoffrost) bestval += 607;
                                    if (discoverCards[i].card.name == CardDB.cardName.razathechained) bestval += 443;
                                    if (discoverCards[i].card.name == CardDB.cardName.razorfenhunter) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.razorpetallasher) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.razorpetalvolley) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.rebuke) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.recklessdiretroll) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.recklessexperimenter) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.recklessflurry) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.recklessrocketeer) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.recombobulator) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.recruiter) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.recurringvillain) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.recycle) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.redbandwasp) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.redemption) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.redmanawyrm) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.refreshmentvendor) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.regenerate) bestval += 54;
                                    if (discoverCards[i].card.name == CardDB.cardName.regeneratinthug) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.reincarnate) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.reliquaryseeker) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.rendblackhand) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.renojackson) bestval += 1373;
                                    if (discoverCards[i].card.name == CardDB.cardName.renotherelicologist) bestval += 383;
                                    if (discoverCards[i].card.name == CardDB.cardName.renouncedarkness) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.repentance) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.replicatingmenace) bestval += 190;
                                    if (discoverCards[i].card.name == CardDB.cardName.researchproject) bestval += 219;
                                    if (discoverCards[i].card.name == CardDB.cardName.restlessmummy) bestval += 30;
                                    if (discoverCards[i].card.name == CardDB.cardName.resurrect) bestval += 159;
                                    if (discoverCards[i].card.name == CardDB.cardName.revenge) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.revengeofthewild) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.rhokdelar) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.rhonin) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.riftcleaver) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.righteouscause) bestval += 70;
                                    if (discoverCards[i].card.name == CardDB.cardName.righteousness) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.righteousprotector) bestval += 122;
                                    if (discoverCards[i].card.name == CardDB.cardName.risingwinds) bestval += 59;
                                    if (discoverCards[i].card.name == CardDB.cardName.riskyskipper) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.ritualchopper) bestval += 104;
                                    if (discoverCards[i].card.name == CardDB.cardName.rivercrocolisk) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.rockbiterweapon) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.rocketboots) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.rockpoolhunter) bestval += 40;
                                    if (discoverCards[i].card.name == CardDB.cardName.rollingfireball) bestval += 65;
                                    if (discoverCards[i].card.name == CardDB.cardName.rollthebones) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.rotface) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.rotnestdrake) bestval += 81;
                                    if (discoverCards[i].card.name == CardDB.cardName.rottenapplebaum) bestval += 128;
                                    if (discoverCards[i].card.name == CardDB.cardName.rumbletuskshaker) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.rumblingelemental) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.rummagingkobold) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.runeforgehaunter) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.runicegg) bestval += 138;
                                    if (discoverCards[i].card.name == CardDB.cardName.rustyrecycler) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.sabotage) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.saboteur) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.sabretoothstalker) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sacredtrial) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.sacrificialpact) bestval += 205;
                                    if (discoverCards[i].card.name == CardDB.cardName.safeguard) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.sahketsapper) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.salhetspride) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.saltydog) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sanctuary) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.sandbinder) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.sandbreath) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.sanddrudge) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.sandhoofwaterbearer) bestval += 55;
                                    if (discoverCards[i].card.name == CardDB.cardName.sandstormelemental) bestval += 132;
                                    if (discoverCards[i].card.name == CardDB.cardName.sandwaspqueen) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.sanguinereveler) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.sap) bestval += 292;
                                    if (discoverCards[i].card.name == CardDB.cardName.saronitechaingang) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.saronitetaskmaster) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.satedthreshadon) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.sathrovarr) bestval += 85;
                                    if (discoverCards[i].card.name == CardDB.cardName.savagecombatant) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.savageroar) bestval += 66;
                                    if (discoverCards[i].card.name == CardDB.cardName.savagery) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.savagestriker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.savannahhighmane) bestval += 26;
                                    if (discoverCards[i].card.name == CardDB.cardName.scalednightmare) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.scalelord) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.scalerider) bestval += 157;
                                    if (discoverCards[i].card.name == CardDB.cardName.scaleworm) bestval += 40;
                                    if (discoverCards[i].card.name == CardDB.cardName.scarabegg) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.scargil) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.scarletcrusader) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.scarletwebweaver) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.scavenginghyena) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.scionofruin) bestval += 98;
                                    if (discoverCards[i].card.name == CardDB.cardName.scorch) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.scorpomatic) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.scourgelordgarrosh) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.screwjankclunker) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.seadevilstinger) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.seaforiumbomber) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.seagiant) bestval += 196;
                                    if (discoverCards[i].card.name == CardDB.cardName.sealfate) bestval += 152;
                                    if (discoverCards[i].card.name == CardDB.cardName.sealofchampions) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.sealoflight) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.seance) bestval += 243;
                                    if (discoverCards[i].card.name == CardDB.cardName.secondratebruiser) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.secretkeeper) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.secretplan) bestval += 33;
                                    if (discoverCards[i].card.name == CardDB.cardName.securethedeck) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.securityrover) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.seepingoozeling) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.selflesshero) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.senjinshieldmasta) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.sensedemons) bestval += 171;
                                    if (discoverCards[i].card.name == CardDB.cardName.sergeantsally) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.serpentegg) bestval += 135;
                                    if (discoverCards[i].card.name == CardDB.cardName.serpentward) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.serratedtooth) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.servantofkalimos) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.servantofyoggsaron) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.sewercrawler) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadeofnaxxramas) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadopanrider) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowascendant) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowblade) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowbolt) bestval += 40;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowbomber) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowboxer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowcaster) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowessence) bestval += 159;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowfiend) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowflame) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowform) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowmadness) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowofdeath) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowrager) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowreaperanduin) bestval += 516;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowsculptor) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowsensei) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowstep) bestval += 274;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowstrike) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowvisions) bestval += 639;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowworddeath) bestval += 537;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowwordhorror) bestval += 55;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowwordpain) bestval += 372;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadowyfigure) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.shadydealer) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.shakyzipgunner) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.shallowgravedigger) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.sharkfinfan) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.shatter) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.shatteredsuncleric) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.shellshifter) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.shieldbearer) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.shieldblock) bestval += 67;
                                    if (discoverCards[i].card.name == CardDB.cardName.shieldbreaker) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.shieldedminibot) bestval += 137;
                                    if (discoverCards[i].card.name == CardDB.cardName.shieldmaiden) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.shieldofgalakrond) bestval += 351;
                                    if (discoverCards[i].card.name == CardDB.cardName.shieldslam) bestval += 61;
                                    if (discoverCards[i].card.name == CardDB.cardName.shifterzerus) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.shiftingscroll) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.shiftingshade) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.shimmerfly) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.shimmeringtempest) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.shipscannon) bestval += 168;
                                    if (discoverCards[i].card.name == CardDB.cardName.shiv) bestval += 37;
                                    if (discoverCards[i].card.name == CardDB.cardName.shootingstar) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.shotbot) bestval += 30;
                                    if (discoverCards[i].card.name == CardDB.cardName.shriek) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.shriekingshroom) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.shrinkmeister) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.shrinkray) bestval += 30;
                                    if (discoverCards[i].card.name == CardDB.cardName.shroombrewer) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.shrubadier) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.shudderwock) bestval += 206;
                                    if (discoverCards[i].card.name == CardDB.cardName.shuma) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.si7agent) bestval += 66;
                                    if (discoverCards[i].card.name == CardDB.cardName.si7infiltrator) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.siamat) bestval += 184;
                                    if (discoverCards[i].card.name == CardDB.cardName.sideshowspelleater) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.siegebreaker) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.siegeengine) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sightlessranger) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.silence) bestval += 286;
                                    if (discoverCards[i].card.name == CardDB.cardName.silentknight) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.silithidswarmer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.siltfinspiritwalker) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.silverbackpatriarch) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.silverhandknight) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.silverhandregent) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.silvermoonguardian) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.silvermoonportal) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.silversword) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.silvervanguard) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.silverwaregolem) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.simulacrum) bestval += 43;
                                    if (discoverCards[i].card.name == CardDB.cardName.sindragosa) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.sinisterdeal) bestval += 91;
                                    if (discoverCards[i].card.name == CardDB.cardName.sinisterstrike) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.siphonsoul) bestval += 100;
                                    if (discoverCards[i].card.name == CardDB.cardName.sirfinleymrrgglton) bestval += 152;
                                    if (discoverCards[i].card.name == CardDB.cardName.sirfinleyofthesands) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.skaterbot) bestval += 150;
                                    if (discoverCards[i].card.name == CardDB.cardName.skelemancer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.skeramcultist) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.skulkinggeist) bestval += 149;
                                    if (discoverCards[i].card.name == CardDB.cardName.skullofthemanari) bestval += 262;
                                    if (discoverCards[i].card.name == CardDB.cardName.skybarge) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.skycapnkragg) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.skyclaw) bestval += 74;
                                    if (discoverCards[i].card.name == CardDB.cardName.skydivinginstructor) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.skyfin) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.skygenralkragg) bestval += 276;
                                    if (discoverCards[i].card.name == CardDB.cardName.skyraider) bestval += 136;
                                    if (discoverCards[i].card.name == CardDB.cardName.skyvateer) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.slam) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.sleepwiththefishes) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.sleepydragon) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.sludgebelcher) bestval += 125;
                                    if (discoverCards[i].card.name == CardDB.cardName.sludgeslurper) bestval += 107;
                                    if (discoverCards[i].card.name == CardDB.cardName.smalltimebuccaneer) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.smalltimerecruits) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.smolderthornlancer) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.smugglerscrate) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.smugglersrun) bestval += 152;
                                    if (discoverCards[i].card.name == CardDB.cardName.sn1psn4p) bestval += 395;
                                    if (discoverCards[i].card.name == CardDB.cardName.snaketrap) bestval += 80;
                                    if (discoverCards[i].card.name == CardDB.cardName.snapfreeze) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.snapjawshellfighter) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sneakydevil) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.sneedsoldshredder) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.snipe) bestval += 41;
                                    if (discoverCards[i].card.name == CardDB.cardName.snowchugger) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.snowflipperpenguin) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.snowfurygiant) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.soggoththeslitherer) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.soldieroffortune) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.solemnvigil) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.sonyashadowdancer) bestval += 26;
                                    if (discoverCards[i].card.name == CardDB.cardName.sootspewer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sorcerersapprentice) bestval += 356;
                                    if (discoverCards[i].card.name == CardDB.cardName.soulfire) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.soulinfusion) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.souloftheforest) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.soulofthemurloc) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.soulwarden) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.soundthebells) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.soupvendor) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.southseacaptain) bestval += 181;
                                    if (discoverCards[i].card.name == CardDB.cardName.southseadeckhand) bestval += 339;
                                    if (discoverCards[i].card.name == CardDB.cardName.southseasquidface) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.sparkdrill) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sparkengine) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sparringpartner) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.spawnofnzoth) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.spawnofshadows) bestval += 295;
                                    if (discoverCards[i].card.name == CardDB.cardName.spectralcutlass) bestval += 49;
                                    if (discoverCards[i].card.name == CardDB.cardName.spectralknight) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.spectralpillager) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.spellbender) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.spellbookbinder) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.spellbreaker) bestval += 211;
                                    if (discoverCards[i].card.name == CardDB.cardName.spellshifter) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.spellslinger) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.spellwardjeweler) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.spellweaver) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.spellzerker) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiderbomb) bestval += 61;
                                    if (discoverCards[i].card.name == CardDB.cardName.spidertank) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.spikedhogrider) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.spikeridgedsteed) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritbomb) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritclaws) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritecho) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritlash) bestval += 539;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritofthebat) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritofthedead) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritofthedragonhawk) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritofthefrog) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritofthelynx) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritoftheraptor) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritoftherhino) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritoftheshark) bestval += 114;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritofthetiger) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.spiritsingerumbra) bestval += 84;
                                    if (discoverCards[i].card.name == CardDB.cardName.spitefulsmith) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.spitefulsummoner) bestval += 30;
                                    if (discoverCards[i].card.name == CardDB.cardName.spittingcamel) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.splintergraft) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.splittingaxe) bestval += 144;
                                    if (discoverCards[i].card.name == CardDB.cardName.splittingfesteroot) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.splittingimage) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.spreadingmadness) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.spreadingplague) bestval += 231;
                                    if (discoverCards[i].card.name == CardDB.cardName.springpaw) bestval += 87;
                                    if (discoverCards[i].card.name == CardDB.cardName.springrocket) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.sprint) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.squallhunter) bestval += 132;
                                    if (discoverCards[i].card.name == CardDB.cardName.squashling) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.squirmingtentacle) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.stablemaster) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.stalagg) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.stampede) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stampedingkodo) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.stampedingroar) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.standagainstdarkness) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.staraligner) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.starfall) bestval += 195;
                                    if (discoverCards[i].card.name == CardDB.cardName.starfire) bestval += 43;
                                    if (discoverCards[i].card.name == CardDB.cardName.stargazerluna) bestval += 518;
                                    if (discoverCards[i].card.name == CardDB.cardName.starvingbuzzard) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.steamsurger) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.steamwheedlesniper) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.steelbeetle) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.steelrager) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stegodon) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stewardofdarkshire) bestval += 78;
                                    if (discoverCards[i].card.name == CardDB.cardName.stitchedtracker) bestval += 63;
                                    if (discoverCards[i].card.name == CardDB.cardName.stolengoods) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.stolensteel) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.stonehilldefender) bestval += 112;
                                    if (discoverCards[i].card.name == CardDB.cardName.stonesentinel) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stoneskinbasilisk) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stoneskingargoyle) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.stonesplintertrogg) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.stonetuskboar) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormchaser) bestval += 29;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormcrack) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormforgedaxe) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormhammer) bestval += 70;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormpikecommando) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormswrath) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormwatcher) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormwindchampion) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.stormwindknight) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stowaway) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.stranglethorntiger) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.streettrickster) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.strengthinnumbers) bestval += 144;
                                    if (discoverCards[i].card.name == CardDB.cardName.strongshellscavenger) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.stubborngastropod) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.subdue) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.subject9) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.suddenbetrayal) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.suddengenesis) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sulthraze) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.summoningportal) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.summoningstone) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.sunbornevalkyr) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.sunfuryprotector) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.sunkeepertarim) bestval += 36;
                                    if (discoverCards[i].card.name == CardDB.cardName.sunreaverspy) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.sunreaverwarmage) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.sunstruckhenchman) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.sunwalker) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.supercollider) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.supremearchaeology) bestval += 99;
                                    if (discoverCards[i].card.name == CardDB.cardName.surgingtempest) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.surrendertomadness) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.swampdragonegg) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.swampkingdred) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.swampleech) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.swampqueenhagatha) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.swarmoflocusts) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.swashburglar) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.swashburglar) bestval += 125;
                                    if (discoverCards[i].card.name == CardDB.cardName.sweepingstrikes) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.swiftmessenger) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.swipe) bestval += 336;
                                    if (discoverCards[i].card.name == CardDB.cardName.swordofjustice) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.sylvanaswindrunner) bestval += 177;
                                    if (discoverCards[i].card.name == CardDB.cardName.taintedzealot) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.taknozwhisker) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.tanarishogchopper) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.tanglefurmystic) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.tarcreeper) bestval += 109;
                                    if (discoverCards[i].card.name == CardDB.cardName.targetdummy) bestval += 91;
                                    if (discoverCards[i].card.name == CardDB.cardName.tarlord) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.tarlurker) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.tastyflyfish) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.taurenwarrior) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.templeberserker) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.templeenforcer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.temporus) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.tendingtauren) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.tentacledmenace) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.tentacleofnzoth) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.tentaclesforarms) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.terrorscalestalker) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.tessgreymane) bestval += 62;
                                    if (discoverCards[i].card.name == CardDB.cardName.testsubject) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.theamazingreno) bestval += 207;
                                    if (discoverCards[i].card.name == CardDB.cardName.thebeast) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.thebeastwithin) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.theblackknight) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.theboomreaver) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.theboomship) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.thecavernsbelow) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.thecurator) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.thedarkness) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.thefistofraden) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.theforestsaid) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.theglassknight) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.thelastkaleidosaur) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.thelichking) bestval += 334;
                                    if (discoverCards[i].card.name == CardDB.cardName.themarshqueen) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.themistcaller) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.therunespear) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.theskeletonknight) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.thesoularium) bestval += 139;
                                    if (discoverCards[i].card.name == CardDB.cardName.thestormbringer) bestval += 112;
                                    if (discoverCards[i].card.name == CardDB.cardName.thevoraxx) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.thingfrombelow) bestval += 190;
                                    if (discoverCards[i].card.name == CardDB.cardName.thistletea) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.thoughtsteal) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.thrallmarfarseer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.thunderbluffvaliant) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.thunderhead) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.thunderlizard) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.ticketscalper) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.tickingabomination) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.tidalsurge) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.timberwolf) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.timeout) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.timerip) bestval += 40;
                                    if (discoverCards[i].card.name == CardDB.cardName.tinkerssharpswordoil) bestval += 53;
                                    if (discoverCards[i].card.name == CardDB.cardName.tinkertowntechnician) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.tinkmasteroverspark) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.tinyknightofevil) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.tipthescales) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.tirionfordring) bestval += 31;
                                    if (discoverCards[i].card.name == CardDB.cardName.togwagglesscheme) bestval += 138;
                                    if (discoverCards[i].card.name == CardDB.cardName.tolvirstoneshaper) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.tolvirwarden) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.tomblurker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.tombpillager) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.tombspider) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.tombwarden) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.tomeofintellect) bestval += 21;
                                    if (discoverCards[i].card.name == CardDB.cardName.tomyside) bestval += 20;
                                    if (discoverCards[i].card.name == CardDB.cardName.toothychest) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.topsyturvy) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.tortollanforager) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.tortollanpilgrim) bestval += 62;
                                    if (discoverCards[i].card.name == CardDB.cardName.tortollanprimalist) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.tortollanshellraiser) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.toshley) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.totemcruncher) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.totemgolem) bestval += 192;
                                    if (discoverCards[i].card.name == CardDB.cardName.totemicmight) bestval += 145;
                                    if (discoverCards[i].card.name == CardDB.cardName.totemicsmash) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.totemicsurge) bestval += 164;
                                    if (discoverCards[i].card.name == CardDB.cardName.tournamentattendee) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.tournamentmedic) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.towncrier) bestval += 115;
                                    if (discoverCards[i].card.name == CardDB.cardName.toxfin) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.toxicarrow) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.toxicologist) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.toxicreinforcements) bestval += 51;
                                    if (discoverCards[i].card.name == CardDB.cardName.toxmonger) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.tracking) bestval += 128;
                                    if (discoverCards[i].card.name == CardDB.cardName.tradeprincegallywix) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.transmogrifier) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.travelinghealer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.treachery) bestval += 30;
                                    if (discoverCards[i].card.name == CardDB.cardName.treenforcements) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.treeoflife) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.treespeaker) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.troggbeastrager) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.trogggloomeater) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.troggzortheearthinator) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.trollbatrider) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.truesilverchampion) bestval += 50;
                                    if (discoverCards[i].card.name == CardDB.cardName.tundrarhino) bestval += 37;
                                    if (discoverCards[i].card.name == CardDB.cardName.tunnelblaster) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.tunneltrogg) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.tuskarrfisherman) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.tuskarrjouster) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.tuskarrtotemic) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.twigoftheworldtree) bestval += 13;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightacolyte) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightdarkmender) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightdrake) bestval += 174;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightelder) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightflamecaller) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightgeomancer) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightguardian) bestval += 93;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightscall) bestval += 130;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightsummoner) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.twilightwhelp) bestval += 55;
                                    if (discoverCards[i].card.name == CardDB.cardName.twinemperorveklor) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.twintyrant) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.twistedknowledge) bestval += 41;
                                    if (discoverCards[i].card.name == CardDB.cardName.twistedworgen) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.twistingnether) bestval += 177;
                                    if (discoverCards[i].card.name == CardDB.cardName.tyrantus) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.ultimateinfestation) bestval += 333;
                                    if (discoverCards[i].card.name == CardDB.cardName.ultrasaur) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.umbralskulker) bestval += 39;
                                    if (discoverCards[i].card.name == CardDB.cardName.unboundelemental) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.underbellyangler) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.underbellyfence) bestval += 64;
                                    if (discoverCards[i].card.name == CardDB.cardName.underbellyooze) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.undercityhuckster) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.undercityvaliant) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.undertaker) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.unearthedraptor) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.unexpectedresults) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.unidentifiedcontract) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.unidentifiedelixir) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.unidentifiedmaul) bestval += 32;
                                    if (discoverCards[i].card.name == CardDB.cardName.unidentifiedshield) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.unitethemurlocs) bestval += 16;
                                    if (discoverCards[i].card.name == CardDB.cardName.unleashthebeast) bestval += 61;
                                    if (discoverCards[i].card.name == CardDB.cardName.unleashthehounds) bestval += 131;
                                    if (discoverCards[i].card.name == CardDB.cardName.unlicensedapothecary) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.unpoweredmauler) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.unpoweredsteambot) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.unsealthevault) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.unseensaboteur) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.unsleepingsoul) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.unstableevolution) bestval += 19;
                                    if (discoverCards[i].card.name == CardDB.cardName.unstableghoul) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.unstableportal) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.untamedbeastmaster) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.untappedpotential) bestval += 84;
                                    if (discoverCards[i].card.name == CardDB.cardName.unwillingsacrifice) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.upgrade) bestval += 102;
                                    if (discoverCards[i].card.name == CardDB.cardName.upgradeableframebot) bestval += 32;
                                    if (discoverCards[i].card.name == CardDB.cardName.upgradedrepairbot) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.ursatron) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.usherofsouls) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.utgardegrapplesniper) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.utheroftheebonblade) bestval += 27;
                                    if (discoverCards[i].card.name == CardDB.cardName.valanyr) bestval += 11;
                                    if (discoverCards[i].card.name == CardDB.cardName.valdrisfelgorge) bestval += 201;
                                    if (discoverCards[i].card.name == CardDB.cardName.valeerathehollow) bestval += 128;
                                    if (discoverCards[i].card.name == CardDB.cardName.validateddoomsayer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.valkyrsoulclaimer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.vanish) bestval += 184;
                                    if (discoverCards[i].card.name == CardDB.cardName.vaporize) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.varianwrynn) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.veiledworshipper) bestval += 33;
                                    if (discoverCards[i].card.name == CardDB.cardName.velenschosen) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.vendetta) bestval += 79;
                                    if (discoverCards[i].card.name == CardDB.cardName.venomancer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.venomizer) bestval += 44;
                                    if (discoverCards[i].card.name == CardDB.cardName.venomstriketrap) bestval += 62;
                                    if (discoverCards[i].card.name == CardDB.cardName.venturecomercenary) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.veranus) bestval += 51;
                                    if (discoverCards[i].card.name == CardDB.cardName.verdantlongneck) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.vereesawindrunner) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.vessina) bestval += 115;
                                    if (discoverCards[i].card.name == CardDB.cardName.vexcrow) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.viciousfledgling) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.viciousscalehide) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.viciousscraphound) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.vilebroodskitterer) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.vilefiend) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.vilefininquisitor) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.vilespineslayer) bestval += 94;
                                    if (discoverCards[i].card.name == CardDB.cardName.vinecleaver) bestval += 57;
                                    if (discoverCards[i].card.name == CardDB.cardName.violethaze) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.violetillusionist) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.violetspellsword) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.violetspellwing) bestval += 403;
                                    if (discoverCards[i].card.name == CardDB.cardName.violetteacher) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.violetwarden) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.violetwurm) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.virmensensei) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.vitalitytotem) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.vividnightmare) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.voidanalyst) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.voidcaller) bestval += 453;
                                    if (discoverCards[i].card.name == CardDB.cardName.voidcontract) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.voidcrusher) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.voidlord) bestval += 484;
                                    if (discoverCards[i].card.name == CardDB.cardName.voidripper) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.voidterror) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.voidwalker) bestval += 54;
                                    if (discoverCards[i].card.name == CardDB.cardName.volatileelemental) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.volcanicdrake) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.volcaniclumberer) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.volcanicpotion) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.volcano) bestval += 82;
                                    if (discoverCards[i].card.name == CardDB.cardName.volcanosaur) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.voljin) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.voltaicburst) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.voodoodoctor) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.voodoodoll) bestval += 35;
                                    if (discoverCards[i].card.name == CardDB.cardName.voodoohexxer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.vryghoul) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.vulgarhomunculus) bestval += 41;
                                    if (discoverCards[i].card.name == CardDB.cardName.vulperascoundrel) bestval += 54;
                                    if (discoverCards[i].card.name == CardDB.cardName.wagglepick) bestval += 25;
                                    if (discoverCards[i].card.name == CardDB.cardName.wailingsoul) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.walkingfountain) bestval += 48;
                                    if (discoverCards[i].card.name == CardDB.cardName.walktheplank) bestval += 37;
                                    if (discoverCards[i].card.name == CardDB.cardName.walnutsprite) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.wanderingmonster) bestval += 139;
                                    if (discoverCards[i].card.name == CardDB.cardName.wanted) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.warbot) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.wardruidloti) bestval += 28;
                                    if (discoverCards[i].card.name == CardDB.cardName.wargear) bestval += 212;
                                    if (discoverCards[i].card.name == CardDB.cardName.wargolem) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.warhorsetrainer) bestval += 70;
                                    if (discoverCards[i].card.name == CardDB.cardName.warmastervoone) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.warpath) bestval += 47;
                                    if (discoverCards[i].card.name == CardDB.cardName.warsongcommander) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.wartbringer) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.wastelandassassin) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.wastelandscorpid) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.waterboy) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.waterelemental) bestval += 22;
                                    if (discoverCards[i].card.name == CardDB.cardName.waxadred) bestval += 17;
                                    if (discoverCards[i].card.name == CardDB.cardName.waxelemental) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.waxmancy) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.weaponizedwasp) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.weaponsproject) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.weaseltunneler) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.webspinner) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.webweave) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.whirliglider) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.whirlingzapomatic) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.whirlkickmaster) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.whirlwind) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.whirlwindtempest) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.whispersofevil) bestval += 84;
                                    if (discoverCards[i].card.name == CardDB.cardName.whiteeyes) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.whizbangthewonderful) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.wickedskeleton) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.wickedwitchdoctor) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.wickerflameburnbristle) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.wildbloodstinger) bestval += 18;
                                    if (discoverCards[i].card.name == CardDB.cardName.wildgrowth) bestval += 37;
                                    if (discoverCards[i].card.name == CardDB.cardName.wildpyromancer) bestval += 120;
                                    if (discoverCards[i].card.name == CardDB.cardName.wildwalker) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.wilfredfizzlebang) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.windfury) bestval += 46;
                                    if (discoverCards[i].card.name == CardDB.cardName.windfuryharpy) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.windshearstormcaller) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.windspeaker) bestval += 3;
                                    if (discoverCards[i].card.name == CardDB.cardName.windupburglebot) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.wingblast) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.wingcommander) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.wingedguardian) bestval += 71;
                                    if (discoverCards[i].card.name == CardDB.cardName.wisp) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.wisperingwoods) bestval += 5;
                                    if (discoverCards[i].card.name == CardDB.cardName.wispsoftheoldgods) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.witchinghour) bestval += 42;
                                    if (discoverCards[i].card.name == CardDB.cardName.witchsapprentice) bestval += 10;
                                    if (discoverCards[i].card.name == CardDB.cardName.witchsbrew) bestval += 14;
                                    if (discoverCards[i].card.name == CardDB.cardName.witchscauldron) bestval += 9;
                                    if (discoverCards[i].card.name == CardDB.cardName.witchwoodapple) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.witchwoodgrizzly) bestval += 24;
                                    if (discoverCards[i].card.name == CardDB.cardName.witchwoodimp) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.witchwoodpiper) bestval += 224;
                                    if (discoverCards[i].card.name == CardDB.cardName.wobblingrunts) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.woecleaver) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.wolfrider) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.woodcuttersaxe) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.worgeninfiltrator) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.worthyexpedition) bestval += 32;
                                    if (discoverCards[i].card.name == CardDB.cardName.wrappedgolem) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.wrath) bestval += 300;
                                    if (discoverCards[i].card.name == CardDB.cardName.wrathguard) bestval += 6;
                                    if (discoverCards[i].card.name == CardDB.cardName.wrathion) bestval += 4;
                                    if (discoverCards[i].card.name == CardDB.cardName.wrenchcalibur) bestval += 63;
                                    if (discoverCards[i].card.name == CardDB.cardName.wretchedreclaimer) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.wretchedtiller) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.wyrmguard) bestval += 8;
                                    if (discoverCards[i].card.name == CardDB.cardName.wyrmrestagent) bestval += 23;
                                    if (discoverCards[i].card.name == CardDB.cardName.wyrmrestpurifier) bestval += 7;
                                    if (discoverCards[i].card.name == CardDB.cardName.youngdragonhawk) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.youngpriestess) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.youthfulbrewmaster) bestval += 56;
                                    if (discoverCards[i].card.name == CardDB.cardName.ysera) bestval += 62;
                                    if (discoverCards[i].card.name == CardDB.cardName.zandalaritemplar) bestval += 2;
                                    if (discoverCards[i].card.name == CardDB.cardName.zap) bestval += 41;
                                    if (discoverCards[i].card.name == CardDB.cardName.zealousinitiate) bestval += 0;
                                    if (discoverCards[i].card.name == CardDB.cardName.zentimo) bestval += 15;
                                    if (discoverCards[i].card.name == CardDB.cardName.zephrysthegreat) bestval += 1404;
                                    if (discoverCards[i].card.name == CardDB.cardName.zerekscloninggallery) bestval += 80;
                                    if (discoverCards[i].card.name == CardDB.cardName.zilliax) bestval += 1550;
                                    if (discoverCards[i].card.name == CardDB.cardName.zolathegorgon) bestval += 467;
                                    if (discoverCards[i].card.name == CardDB.cardName.zombiechow) bestval += 12;
                                    if (discoverCards[i].card.name == CardDB.cardName.zoobot) bestval += 1;
                                    if (discoverCards[i].card.name == CardDB.cardName.zuldrakritualist) bestval += 128;
                                    if (discoverCards[i].card.name == CardDB.cardName.zuljin) bestval += 76;
                                    if (discoverCards[i].card.name == CardDB.cardName.zzerakuthewarped) bestval += 21;


                                    break;
                                case GAME_TAG.ADAPT:
                                    bool found = false;
                                    foreach (Minion m in tmpPlf.ownMinions)
                                    {
                                        if (m.entitiyID == sourceEntityId)
                                        {
                                            bool forbidden = false;
                                            switch (discoverCards[i].card.cardIDenum)
                                            {
                                                case CardDB.cardIDEnum.UNG_999t5: if (m.cantBeTargetedBySpellsOrHeroPowers) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t6: if (m.taunt) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t7: if (m.windfury) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t8: if (m.divineshild) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t10: if (m.stealth) forbidden = true; break;
                                                case CardDB.cardIDEnum.UNG_999t13: if (m.poisonous) forbidden = true; break;
                                            }
                                            if (forbidden) bestval = -2000000;
                                            else
                                            {
                                                discoverCards[i].card.sim_card.onCardPlay(tmpPlf, true, m, 0);
                                                bestval = ai.mainTurnSimulator.doallmoves(tmpPlf);
                                            }
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found) Log.ErrorFormat("[AI] sourceEntityId is missing");
                                    break;
                            }
                            bestval += discoverCards[i].card.sim_card.getDiscoverScore(tmpPlf);
                            if (bestDiscoverValue <= bestval)
                            {
                                bestDiscoverValue = bestval;
                                dirtychoice = i;
                            }
                        }
                    }
                    ai.mainTurnSimulator.setSecondTurnSimu(true, dirtyTwoTurnSim);
                }
                if (sourceEntityCId == CardDB.cardIDEnum.UNG_035) dirtychoice = new Random().Next(0, 2);
                //if (dirtychoice == 0) dirtychoice = 1;
                //else if (dirtychoice == 1) dirtychoice = 0;
                int ttf = (int)(DateTime.Now - tmp).TotalMilliseconds;
                Helpfunctions.Instance.logg("发现卡牌: " + dirtychoice + (discoverCardsCount > 1 ? " " + discoverCards[1].card.cardIDenum : "") + (discoverCardsCount > 0 ? " " + discoverCards[0].card.cardIDenum : "") + (discoverCardsCount > 2 ? " " + discoverCards[2].card.cardIDenum : ""));
                if (ttf < 3000) return (new Random().Next(ttf < 1300 ? 1300 - ttf : 0, 3100 - ttf));
            }
            else
            {
                Helpfunctions.Instance.logg("选择这张卡牌: " + dirtychoice);
                return (new Random().Next(1100, 3200));
            }
            return 0;
        }

        /// <summary>
        /// Under construction.
        /// </summary>
        /// <returns></returns>
        public async Task OpponentTurnLogic()
        {
            Log.InfoFormat("[对手回合]");


        }

        #endregion

        #region ArenaDraft

        /// <summary>
        /// Under construction.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task ArenaDraftLogic(ArenaDraftData data)
        {
            Log.InfoFormat("[ArenaDraft]");

            // We don't have a hero yet, so choose one.
            if (data.Hero == null)
            {
                Log.InfoFormat("[ArenaDraft] Hero: [{0} ({3}) | {1} ({4}) | {2} ({5})].",
                    data.Choices[0].EntityDef.CardId, data.Choices[1].EntityDef.CardId, data.Choices[2].EntityDef.CardId,
                    data.Choices[0].EntityDef.Name, data.Choices[1].EntityDef.Name, data.Choices[2].EntityDef.Name);

                // Quest support logic!
                var questIds = TritonHs.CurrentQuests.Select(q => q.Id).ToList();
                foreach (var choice in data.Choices)
                {
                    var @class = choice.EntityDef.Class;
                    foreach (var questId in questIds)
                    {
                        if (TritonHs.IsQuestForClass(questId, @class))
                        {
                            data.Selection = choice;
                            Log.InfoFormat(
                                "[ArenaDraft] Choosing hero \"{0}\" because it matches a current quest.",
                                data.Selection.EntityDef.Name);
                            return;
                        }
                    }
                }

                // TODO: I'm sure there's a better way to do this, but w/e, no time to waste right now.

                // #1
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass1)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the first preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // #2
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass2)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the second preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // #3
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass3)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the third preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // #4
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass4)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the fourth preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // #5
                foreach (var choice in data.Choices)
                {
                    if ((TAG_CLASS)choice.EntityDef.Class == DefaultRoutineSettings.Instance.ArenaPreferredClass5)
                    {
                        data.Selection = choice;
                        Log.InfoFormat(
                            "[ArenaDraft] Choosing hero \"{0}\" because it matches the fifth preferred arena class.",
                            data.Selection.EntityDef.Name);
                        return;
                    }
                }

                // Choose a random hero.
                data.RandomSelection();

                Log.InfoFormat(
                    "[ArenaDraft] Choosing hero \"{0}\" because no other preferred arena classes were available.",
                    data.Selection.EntityDef.Name);

                return;
            }

            // Normal card choices.
            Log.InfoFormat("[ArenaDraft] Card: [{0} ({3}) | {1} ({4}) | {2} ({5})].", data.Choices[0].EntityDef.CardId,
                data.Choices[1].EntityDef.CardId, data.Choices[2].EntityDef.CardId, data.Choices[0].EntityDef.Name,
                data.Choices[1].EntityDef.Name, data.Choices[2].EntityDef.Name);

            var actor =
                data.Choices.Where(c => ArenavaluesReader.Get.ArenaValues.ContainsKey(c.EntityDef.CardId))
                    .OrderByDescending(c => ArenavaluesReader.Get.ArenaValues[c.EntityDef.CardId]).FirstOrDefault();
            if (actor != null)
            {
                data.Selection = actor;
            }
            else
            {
                data.RandomSelection();
            }
        }

        #endregion

        #region Handle Quests

        /// <summary>
        /// Under construction.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task HandleQuestsLogic(QuestData data)
        {
            Log.InfoFormat("[处理日常任务]");

            // Loop though all quest tiles.
            foreach (var questTile in data.QuestTiles)
            {
                // If we can't cancel a quest, we shouldn't try to.
                if (questTile.IsCancelable)
                {
                    if (DefaultRoutineSettings.Instance.QuestIdsToCancel.Contains(questTile.Achievement.Id))
                    {
                        // Mark the quest tile to be canceled.
                        questTile.ShouldCancel = true;

                        StringBuilder questsInfo = new StringBuilder("", 1000);
                        questsInfo.Append("[处理日常任务] 任务列表: ");
                        int qNum = data.QuestTiles.Count;
                        for (int i = 0; i < qNum; i++)
                        {
                            var q = data.QuestTiles[i].Achievement;
                            if (q.RewardData.Count > 0)
                            {
                                questsInfo.Append("[").Append(q.RewardData[0].Count).Append("x ").Append(q.RewardData[0].Type).Append("] ");
                            }
                            questsInfo.Append(q.Name);
                            if (i < qNum - 1) questsInfo.Append(", ");
                        }
                        questsInfo.Append(". 尝试取消任务: ").Append(questTile.Achievement.Name);
                        Log.InfoFormat(questsInfo.ToString());
                        await Coroutine.Sleep(new Random().Next(4000, 8000));
                        return;
                    }
                }
                else if (DefaultRoutineSettings.Instance.QuestIdsToCancel.Count > 0)
                {
                    Log.InfoFormat("取消任务失败.");
                }
            }
        }

        #endregion

        #endregion

        #region Override of Object

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + ": " + Description;
        }

        #endregion

        private void GameEventManagerOnGameOver(object sender, GameOverEventArgs gameOverEventArgs)
        {
            Log.InfoFormat("[游戏结束] {0}{2} => {1}.", gameOverEventArgs.Result,
                GameEventManager.Instance.LastGamePresenceStatus, gameOverEventArgs.Conceded ? " [conceded]" : "");
        }

        private void GameEventManagerOnNewGame(object sender, NewGameEventArgs newGameEventArgs)
        {

        }

        private void GameEventManagerOnQuestUpdate(object sender, QuestUpdateEventArgs questUpdateEventArgs)
        {
            Log.InfoFormat("[任务刷新]");
            foreach (var quest in TritonHs.CurrentQuests)
            {
                Log.InfoFormat("[任务刷新][{0}]{1}: {2} ({3} / {4}) [{6}x {5}]", quest.Id, quest.Name, quest.Description, quest.CurProgress,
                    quest.MaxProgress, quest.RewardData[0].Type, quest.RewardData[0].Count);
            }
        }

        private void GameEventManagerOnArenaRewards(object sender, ArenaRewardsEventArgs arenaRewardsEventArgs)
        {
            Log.InfoFormat("[竞技场奖励]");
            foreach (var reward in arenaRewardsEventArgs.Rewards)
            {
                Log.InfoFormat("[竞技场奖励] {1}x {0}.", reward.Type, reward.Count);
            }
        }

        private HSCard getEntityWithNumber(int number)
        {
            foreach (HSCard e in getallEntitys())
            {
                if (number == e.EntityId) return e;
            }
            return null;
        }

        private HSCard getCardWithNumber(int number)
        {
            foreach (HSCard e in getallHandCards())
            {
                if (number == e.EntityId) return e;
            }
            return null;
        }

        private List<HSCard> getallEntitys()
        {
            var result = new List<HSCard>();
            HSCard ownhero = TritonHs.OurHero;
            HSCard enemyhero = TritonHs.EnemyHero;
            HSCard ownHeroAbility = TritonHs.OurHeroPowerCard;
            List<HSCard> list2 = TritonHs.GetCards(CardZone.Battlefield, true);
            List<HSCard> list3 = TritonHs.GetCards(CardZone.Battlefield, false);

            result.Add(ownhero);
            result.Add(enemyhero);
            result.Add(ownHeroAbility);

            result.AddRange(list2);
            result.AddRange(list3);

            return result;
        }

        private List<HSCard> getallHandCards()
        {
            List<HSCard> list = TritonHs.GetCards(CardZone.Hand, true);
            return list;
        }
    }
}
