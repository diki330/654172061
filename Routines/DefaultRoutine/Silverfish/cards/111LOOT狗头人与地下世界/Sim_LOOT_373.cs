using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_373 : SimTemplate //治疗之雨
    {

        //    恢复12点生命值,随机分配到所有友方角色上.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int heal = (ownplay) ? p.getSpellHeal(4) : p.getEnemySpellHeal(4);
            p.minionGetDamageOrHeal(p.ownHero, -heal);
        }

    }
}