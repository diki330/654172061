using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_EX1_195  : SimTemplate// EX1_195  库尔提拉斯教士
	  //战吼：使一个友方随从获得+2生命值。
	{
		public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
		{
			if (target != null) p.minionGetBuffed(target, 0, 2);
		}
	}
}