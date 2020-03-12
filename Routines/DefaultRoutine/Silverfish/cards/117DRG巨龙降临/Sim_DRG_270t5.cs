using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_270t5 : SimTemplate //* 玛里苟斯的冰霜新星(Malygos's Nova)
	{
		// [x]Freeze all enemy minions.
		// 冻结所有敌方随从。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp = (ownplay) ? p.enemyMinions : p.ownMinions;
            foreach (Minion t in temp)
            {
                p.minionGetFrozen(t);
            }
        }
	}
}