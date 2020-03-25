using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_368 : SimTemplate //* 虚空领主
    {
        //Deathrattle: Summon three 1/3 voidwalker.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_065); //voidwalker

        public override void onDeathrattle(Playfield p, Minion m)
        {
            p.callKid(kid, m.zonepos - 1, m.own);
            p.callKid(kid, m.zonepos - 1, m.own);
            p.callKid(kid, m.zonepos - 1, m.own);
        }
    }
}