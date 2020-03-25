using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_161 : SimTemplate //* 食肉魔块
    {
        //战吼:消灭一个友方随从.亡语:召唤被消灭随从的两个复制.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                p.LurkersDB.Add(own.entitiyID, new IDEnumOwner() { IDEnum = target.handcard.card.cardIDenum, own = target.own });
                p.minionGetDestroyed(target);
            }
        }

        public override void onDeathrattle(Playfield p, Minion m)
        {
            if (p.LurkersDB.ContainsKey(m.entitiyID))
            {
                bool own = p.LurkersDB[m.entitiyID].own;
                int pos = own ? p.ownMinions.Count : p.enemyMinions.Count;
                p.callKid(CardDB.Instance.getCardDataFromID(p.LurkersDB[m.entitiyID].IDEnum), pos, own);
                p.callKid(CardDB.Instance.getCardDataFromID(p.LurkersDB[m.entitiyID].IDEnum), pos, own);
            }
        }
    }
}