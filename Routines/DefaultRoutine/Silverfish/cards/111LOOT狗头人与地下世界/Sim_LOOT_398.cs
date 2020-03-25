using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_398 : SimTemplate //和蔼的灯神
    {

        //    At the end of your turn, restore 3 Health to your hero.

        public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            if (triggerEffectMinion.own == turnEndOfOwner)
            {

                if (triggerEffectMinion.own)
                {
                    int heal = p.getMinionHeal(3);
                    p.minionGetDamageOrHeal(p.ownHero, -heal, true);
                }
                else
                {
                    int heal = p.getEnemyMinionHeal(3);
                    p.minionGetDamageOrHeal(p.enemyHero, -heal, true);
                }

            }
        }


    }

}