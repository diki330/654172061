using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_EX1_198  : SimTemplate// EX1_198  娜塔莉·塞林
	  //战吼：消灭一个随从并获得其生命值。
	{
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (target != null)
			{
				own.Hp = target.Hp;
				p.minionGetDestroyed(target);
			}
		}
	}
}