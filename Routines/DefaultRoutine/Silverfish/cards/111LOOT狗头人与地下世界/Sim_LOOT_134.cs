using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_134 : SimTemplate //* 利齿宝箱
    {
        //At the start of your turn, set this minion's Attack to 4.

        public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (triggerEffectMinion.own == turnStartOfOwner)
            {
                if (triggerEffectMinion.Angr != 4) triggerEffectMinion.Angr = 4;
            }
        }
    }
}