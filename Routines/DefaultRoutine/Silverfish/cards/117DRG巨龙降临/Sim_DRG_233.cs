using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_233 : SimTemplate  //by Summer Mate
    {


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 1, 2);

            bool dragonInHand = false;
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
                {
                    dragonInHand = true;
                    break;
                }
            }
            if (dragonInHand) target.divineshild = true;
        }


    }
}
