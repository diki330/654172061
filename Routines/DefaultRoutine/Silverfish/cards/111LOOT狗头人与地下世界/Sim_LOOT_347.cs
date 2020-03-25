using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_347 : SimTemplate //* 狗头人学徒
    {
        // Battlecry: Deal 3 damage randomly split among all enemies.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                if ((TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL)
                {
                    p.allCharsOfASideGetRandomDamage(!own.own, 3);
                    break;
                }
            }
        }
    }
}