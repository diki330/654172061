using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_417 : SimTemplate //* 大灾变
    {
        //Destroy all other minions and discard your hand.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int ownanz = p.ownMinions.Count;
            int enemanz = p.enemyMinions.Count;
            p.allMinionsGetDestroyed();
            p.discardCards(10, ownplay);
        }
    }
}