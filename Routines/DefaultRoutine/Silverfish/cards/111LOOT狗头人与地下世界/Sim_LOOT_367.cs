using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_367 : SimTemplate //* 枯须铸甲师
    {
        // 战吼:每有一个敌方随从,便获得2点护甲值.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                do
                {
                    p.minionGetArmor(own.own ? p.ownHero : p.enemyHero, 2);
                }
                while (p.enemyMinions.Count > p.ownMinions.Count);
            }
        }
    }
}