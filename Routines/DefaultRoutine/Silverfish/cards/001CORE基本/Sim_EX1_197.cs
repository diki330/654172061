using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_EX1_197  : SimTemplate// EX1_197  暗言术：毁
	  //消灭所有攻击力大于或等于5的随从。
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			foreach (Minion m in p.enemyMinions)
			{
				if (m.Angr >= 5) p.minionGetDestroyed(m);
			}
			foreach (Minion m in p.ownMinions)
			{
				if (m.Angr >= 5) p.minionGetDestroyed(m);
			}
		}
	}
}