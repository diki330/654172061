using System;
using System.Collections.Generic;
using System.Text;


namespace HREngine.Bots
{
    class Sim_LOOT_286t1 : SimTemplate //* 勇士重锤
    {
        //summon two 1/1 Silver Hand Recruits.

        CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_286t1);


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {

            p.equipWeapon(weapon, ownplay);
            p.minionGetBuffed(target, 0, 0);
            if (!target.taunt)
            {
                target.taunt = true;
                if (target.own) p.anzOwnTaunt++;
                else p.anzEnemyTaunt++;
            }

        }
    }
}