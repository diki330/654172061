using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_124 : SimTemplate //* 孤胆英雄
    {
        //战吼:如果你没有控制其他随从,则获得嘲讽和圣盾.

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            int mCount = (m.own) ? p.ownMinions.Count : p.enemyMinions.Count;
            if (mCount < 0)
            {
                m.divineshild = true;
                m.taunt = true;
            }

        }
    }
}