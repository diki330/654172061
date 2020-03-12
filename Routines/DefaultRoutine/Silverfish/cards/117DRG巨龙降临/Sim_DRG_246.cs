using System;
using System.Collections.Generic;
using System.Text;


namespace HREngine.Bots
{

	class Sim_DRG_246 : SimTemplate //* 时空裂痕 Time Rip
	{
		//Destroy a minion.<b>Invoke</b> Galakrond.
		//消灭一个随从。<b>祈求</b>迦拉克隆。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.minionGetDestroyed(target);
			p.getGalakrondInvoke(ownplay);
		}



	}
}