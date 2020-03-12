using System.Collections.Generic;

namespace HREngine.Bots
{
	class Sim_ULD_197 : SimTemplate //* 流沙元素 Quicksand Elemental
	{
        //<b>Battlecry:</b> Give all enemy minions -2 Attack this turn.
        //<b>战吼：</b>在本回合中，使所有敌方随从获得-2攻击力。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.enemyMinions : p.ownMinions;
            foreach (Minion m in temp)
            {
                p.minionGetTempBuff(m, -2, 0);
            }
        }

    }
}