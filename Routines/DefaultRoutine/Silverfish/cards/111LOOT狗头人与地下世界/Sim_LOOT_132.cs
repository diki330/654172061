using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_132 : SimTemplate //* 屠龙者
    {
        //战吼:对一条龙造成6点伤害.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                int dmg = 6;
                p.minionGetDamageOrHeal(target, dmg);
            }
        }
    }
}