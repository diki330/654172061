using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_365 : SimTemplate //* 宝石魔像
    {
        //Taunt Can only attack if you have 5 or more Armor.

        public override void onMinionWasSummoned(Playfield p, Minion m, Minion summonedMinion)
        {
            if (!m.silenced)
            {
                m.cantAttack = (p.ownHero.armor < 5) ? true : false;
                m.updateReadyness();
            }
        }
    }
}