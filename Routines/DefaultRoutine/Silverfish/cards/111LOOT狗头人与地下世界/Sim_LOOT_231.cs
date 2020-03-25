using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_231 : SimTemplate //奥术工匠
    {

        //    每当你施放一个法术,便获得等同于其法力值消耗的护甲值.

        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool wasOwnCard, Minion triggerEffectMinion)
        {
            if (hc.card.type == CardDB.cardtype.SPELL && wasOwnCard == triggerEffectMinion.own)
            {
                p.minionGetArmor(triggerEffectMinion.own ? p.ownHero : p.enemyHero, 2);

            }

        }

    }
}