using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_111 : SimTemplate //机械异种蝎
    {
        //战吼:消灭一个攻击力小于或等于1的随从.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp2 = (own.own) ? new List<Minion>(p.enemyMinions) : new List<Minion>(p.ownMinions);
            temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//destroys the weakest
            foreach (Minion enemy in temp2)
            {
                if (enemy.Angr <= 1)
                {
                    p.minionGetDestroyed(enemy);
                    break;
                }
                if (own.Angr <= 1)
                {
                    p.minionGetDestroyed(own);
                    break;
                }
            }
        }
    }
}
