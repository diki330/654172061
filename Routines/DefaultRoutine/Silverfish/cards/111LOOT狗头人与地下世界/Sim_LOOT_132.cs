using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_132 : SimTemplate //* ������
    {
        //ս��:��һ�������6���˺�.

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