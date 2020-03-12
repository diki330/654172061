using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_095 : SimTemplate //* 维拉努斯 Veranus
	{
		//<b>Battlecry:</b> Change the Health of all enemy minions to 1.
		//<b>战吼：</b>将所有敌方随从的生命值变为1。
        
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (own.own)
			{
				foreach (Minion m in p.enemyMinions)
				{
					p.minionSetLifetoX(m, 1);
				}
			}
			else
			{
				foreach (Minion m in p.ownMinions)
				{
					p.minionSetLifetoX(m, 1);
				}
			}				
		}


	}
}