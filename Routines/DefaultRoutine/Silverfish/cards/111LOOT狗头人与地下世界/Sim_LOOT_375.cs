using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_375 : SimTemplate //公会招募员
    {

        //    战吼:招募一个法力值消耗小于或等于4的随从.
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            int pos = (own.own) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(p.getRandomCardForManaMinion(4), pos, own.own, false);
        }


    }
}