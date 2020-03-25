using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_153 : SimTemplate //* 紫色岩虫
    {
        //Deathrattle: Summon 7个 1/1 Murlocs.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.UNG_201t); //Primalfin

        public override void onDeathrattle(Playfield p, Minion m)
        {
            p.callKid(kid, m.zonepos - 1, m.own);
            p.callKid(kid, m.zonepos - 1, m.own);
            p.callKid(kid, m.zonepos - 1, m.own);
            p.callKid(kid, m.zonepos - 1, m.own);
            p.callKid(kid, m.zonepos - 1, m.own);
            p.callKid(kid, m.zonepos - 1, m.own);
            p.callKid(kid, m.zonepos - 1, m.own);
        }
    }
}