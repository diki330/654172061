using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_144 : SimTemplate //藏宝巨龙
    {

        //    kampfschrei:/ gebt eurem gegner 2 Coin.
        public override void onDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardName.thecoin, !m.own);
            p.drawACard(CardDB.cardName.thecoin, !m.own);
        }


    }
}