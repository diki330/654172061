using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_537 : SimTemplate //* 魔网操控者
    {
        // 战吼:如果你的手牌中有你的套牌之外的牌,则这些牌的法力值消耗减少2点

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                int heroClass = (int)p.ownHeroStartClass;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.Class != heroClass && hc.card.Class != 0) hc.manacost = Math.Max(0, hc.manacost - 2);
                }
            }
        }
    }
}