using System;
using System.Collections.Generic;
using System.Text;


namespace HREngine.Bots
{
    class Sim_LOOT_286t4 : SimTemplate //* 净化重锤
    {
        //Give your minions divine shield.


        CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_286t4);


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(weapon, ownplay);
            p.minionGetBuffed(target, 0, 0);
            target.divineshild = true;

        }
    }
}
