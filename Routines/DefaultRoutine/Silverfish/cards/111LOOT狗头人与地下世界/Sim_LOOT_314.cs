using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_314 : SimTemplate //* 灰熊守护者
    {
        //Deathrattle: Recruit 2 minions that cost (4) or less.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_048);

        public override void onDeathrattle(Playfield p, Minion m)
        {
            int pos = (m.own) ? p.ownMinions.Count : p.enemyMinions.Count;

            p.callKid(kid, pos, m.own, false);
            p.callKid(kid, pos, m.own);
        }
    }
}