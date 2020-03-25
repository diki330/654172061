using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_504 : SimTemplate //* 不稳定的异变
    {
        //Transform a friendly minion into a random one that costs (1) more.在本回合可以重复使用.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (target == null) return;
            p.minionTransform(target, p.getRandomCardForManaMinion(target.handcard.card.cost + 1));
            p.drawACard(CardDB.cardIDEnum.GIL_506, ownplay, true);
        }

        public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            p.discardCards(1, turnEndOfOwner);
        }
    }
}