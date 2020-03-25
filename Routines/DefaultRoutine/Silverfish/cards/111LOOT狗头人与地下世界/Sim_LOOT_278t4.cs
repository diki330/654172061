using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_278t4 : SimTemplate //* 希望药剂
    {
        //使一个随从获得+2/+2,以及亡语:将该随从移回你的手牌

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 2, 2);
            target.desperatestand++;
        }
    }
}
