using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_516 : SimTemplate //蛇发女妖佐拉
    {

        //    战吼:选择一个友方随从,将它的金色复制置入你的手牌.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null) p.drawACard(target.handcard.card.name, own.own, true);
        }


    }
}