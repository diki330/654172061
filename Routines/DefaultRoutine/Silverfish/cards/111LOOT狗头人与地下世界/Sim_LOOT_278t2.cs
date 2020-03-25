using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_278t2 : SimTemplate //* 纯净药剂
    {
        //使一个随从获得+2/+2,在你手牌中时获得额外圣盾效果

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 2, 2);
            target.divineshild = true;
        }
    }
}
