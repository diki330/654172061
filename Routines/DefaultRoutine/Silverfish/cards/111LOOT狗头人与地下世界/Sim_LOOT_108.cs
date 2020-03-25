using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_108 : SimTemplate //* 艾露尼斯
    {
        // At the end of your turn, draw 3 cards.

        CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_108);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(weapon, ownplay);
            int cardDemands = Math.Max(0, 4 - p.owncards.Count);
            if (ownplay) p.evaluatePenality -= cardDemands * 20;
        }

        public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            if (triggerEffectMinion.own == turnEndOfOwner)
            {
                p.drawACard(CardDB.cardName.unknown, turnEndOfOwner);
                p.drawACard(CardDB.cardName.unknown, turnEndOfOwner);
                p.drawACard(CardDB.cardName.unknown, turnEndOfOwner);
            }
        }
    }
}