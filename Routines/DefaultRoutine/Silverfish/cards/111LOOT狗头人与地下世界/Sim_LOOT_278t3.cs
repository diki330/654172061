using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_278t3 : SimTemplate //* 暗影药剂
    {
        //使一个随从获得+2/+2,并召唤一个1/1复制

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 2, 2);
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;
            int pos = temp.Count;
            if (pos < 7)
            {
                p.callKid(target.handcard.card, pos, ownplay);
                temp[pos].setMinionToMinion(target);
                p.ownMinions[pos].Angr = 1;
                p.ownMinions[pos].Hp = 1;
                p.ownMinions[pos].maxHp = 1;
            }
        }
    }
}
