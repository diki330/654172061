using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_099t3 : SimTemplate //* 统御 Domination
    {
        //Give your other minions +2/+2.
        //使你的所有其他随从获得+2/+2。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.allMinionOfASideGetBuffed(ownplay, 2, 2);
        }

    }
}