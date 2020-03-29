using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_EX1_194  : SimTemplate// EX1_194  能量灌注
	  //使一个随从获得+2/+6。
	{
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.minionGetBuffed(target, 2, 6);
		}
	}
}