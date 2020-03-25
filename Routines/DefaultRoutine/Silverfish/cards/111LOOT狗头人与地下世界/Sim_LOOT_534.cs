using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_534 : SimTemplate //* 镀金的石像鬼
    {
        //Deathrattle: Put a Coin into your hand.

        public override void onDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardName.thecoin, m.own);
        }
    }
}