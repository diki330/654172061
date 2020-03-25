using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_383 : SimTemplate //* 饥饿的双头怪
    {
        // Battlecry: Summon a random 2-Cost minion for your opponent.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_120); //rivercrocolisk

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            int zonepos = (m.own) ? p.enemyMinions.Count : p.ownMinions.Count;
            p.callKid(kid, zonepos, !m.own);
        }
    }
}