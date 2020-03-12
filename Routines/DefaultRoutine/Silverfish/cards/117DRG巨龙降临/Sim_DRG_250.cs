using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_250 : SimTemplate //* 邪鬼仪式 Fiendish Rites
	{
		//<b>Invoke</b> Galakrond.Give your minions +1 Attack.
		//<b>祈求</b>迦拉克隆。使你的所有随从获得+1攻击力。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.getGalakrondInvoke(ownplay);
			p.allMinionOfASideGetBuffed(ownplay, 1, 0);
		}

	}
}