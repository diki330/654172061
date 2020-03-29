using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_EX1_196  : SimTemplate// EX1_196  血色征服者
	  //战吼：直到你的下个回合，使一个敌方随从获得-2攻击力。
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				p.minionGetTempBuff(target, -2, 0);
			}
		}
	}
}