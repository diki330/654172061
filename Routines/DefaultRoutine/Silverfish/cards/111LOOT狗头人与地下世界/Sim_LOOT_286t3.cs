using System;
using System.Collections.Generic;
using System.Text;


namespace HREngine.Bots
{
    class Sim_LOOT_286t3 : SimTemplate //* 祝福重锤
    {
        //Give your minions +1/+1.

        CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_286t3);


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(weapon, ownplay);
            p.allMinionOfASideGetBuffed(ownplay, 1, 1);

        }
    }
}