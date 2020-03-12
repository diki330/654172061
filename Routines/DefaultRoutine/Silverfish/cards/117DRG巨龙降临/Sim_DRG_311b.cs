using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots {
    class Sim_DRG_311b : SimTemplate {
        //简单维修 +2生命值和嘲讽
        public override void onCardPlay (Playfield p, bool ownplay, Minion target, int choice) {
            p.minionGetBuffed (target, 0, 2);
            if (!target.taunt) {
                target.taunt = true;
                if (target.own) p.anzOwnTaunt++;
                else p.anzEnemyTaunt++;
            }
        }

    }
}