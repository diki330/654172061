using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_539 : SimTemplate //* 恶毒的召唤师
    {

        // 战吼：</b>揭示你牌库中的一张法术牌。随机召唤一个法力值消耗与其相同的随从.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_187); //

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            List<Minion> list = (m.own) ? p.ownMinions : p.enemyMinions;
            int anz = list.Count;
            p.callKid(kid, m.zonepos, m.own);
            if (anz < 7 && !list[m.zonepos].taunt)
            {
                list[m.zonepos].taunt = true;
                if (m.own) p.anzOwnTaunt++;
                else p.anzEnemyTaunt++;
            }
        }
    }
}