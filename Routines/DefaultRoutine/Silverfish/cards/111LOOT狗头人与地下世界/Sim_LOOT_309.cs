using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_309 : SimTemplate //* 橡树的召唤
    {
        //Gain 6 Armor. Recruit a minion that cost (4) or less.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_048);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                p.minionGetArmor(p.ownHero, 6);
            }
            else
            {
                p.minionGetArmor(p.enemyHero, 6);
            }
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, pos, ownplay, false);
        }
    }
}