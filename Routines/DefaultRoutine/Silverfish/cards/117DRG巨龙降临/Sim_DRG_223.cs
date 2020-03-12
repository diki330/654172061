using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_223 : SimTemplate //* Fire Plume Phoenix
	{
		//<b>战吼：</b>如果你有<b>过载</b>的法力水晶，造成5点伤害。

		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{ 
		  
		   if (p.ueberladung > 0)
		   
		    {
			   p.minionGetDamageOrHeal(target, 5);
		    }
		  
		}
	}
}