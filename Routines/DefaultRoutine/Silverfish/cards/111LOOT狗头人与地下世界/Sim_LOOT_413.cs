using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_413 : SimTemplate //* 硬壳甲虫
    {
        // Deathrattle: Gain 3 Armor.

        public override void onDeathrattle(Playfield p, Minion m)
        {
            p.minionGetArmor(m.own ? p.ownHero : p.enemyHero, 3);
        }
    }
}