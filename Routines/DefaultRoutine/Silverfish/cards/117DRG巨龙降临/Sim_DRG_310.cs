using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots {
    class Sim_DRG_310 : SimTemplate {
        //辟法龙人
        //无法成为技能或法术的目标
        public override void getBattlecryEffect (Playfield p, Minion own, Minion target, int choice) {
            own.cantBeTargetedBySpellsOrHeroPowers = true;
        }

    }
}