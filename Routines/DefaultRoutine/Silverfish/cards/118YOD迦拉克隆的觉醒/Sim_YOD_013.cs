using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_YOD_013 : SimTemplate
    {
        //龙鳞祭司 
        //战吼：如果你的手牌中有龙牌，便发现你牌库中的一张法术牌。

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                bool dragonInHand = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
                    {
                        dragonInHand = true;
                        break;
                    }
                }
                if (dragonInHand) p.drawACard(CardDB.cardName.unknown, own.own);
            }
        }

    }
}
