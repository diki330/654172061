using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_278t1 : SimTemplate //* 生命药剂
    {
        //使一个随从获得+2/+2,在你手牌中时获得额外吸血效果

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 2, 2);
            target.lifesteal = true;
        }
    }
}
