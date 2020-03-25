using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_529 : SimTemplate //* 虚空撕裂者
    {
        // Battlecry: Swap the Attack and Health of all minion.

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null) p.minionSwapAngrAndHP(target);
        }
    }
}