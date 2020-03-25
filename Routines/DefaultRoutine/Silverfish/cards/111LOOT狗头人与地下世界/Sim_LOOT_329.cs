using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_329 : SimTemplate //* 伊克斯里德真菌之王
    {
        //在你使用一张随从牌后,召唤一个该随从的复制

        public override void onMinionWasSummoned(Playfield p, Minion m, Minion summonedMinion)
        {
            int pos = (m.own) ? p.ownMinions.Count : p.enemyMinions.Count;
            if (pos < 7)
            {
                p.callKid(summonedMinion.handcard.card, pos, m.own);
                p.ownMinions[pos].setMinionToMinion(summonedMinion);
            }
        }
    }
}