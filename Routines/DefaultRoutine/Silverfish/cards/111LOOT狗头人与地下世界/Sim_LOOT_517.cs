using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_517 : SimTemplate //* 低语元素
    {
        // 战吼:你在本回合使用的下一张战吼牌将触发两次.

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (m.own) p.ownBrannBronzebeard++;
        }
    }
}