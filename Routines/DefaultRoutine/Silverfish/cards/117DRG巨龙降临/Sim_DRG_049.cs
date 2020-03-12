using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_049 : SimTemplate //* 美味飞鱼 Tasty Flyfish
	{
		//<b>Deathrattle:</b> Give a Dragon in your hand +2/+2.
		//<b>亡语：</b>使你手牌中的一张龙牌获得+2/+2。
		public override void onDeathrattle(Playfield p, Minion m)
		{
			if (m.own)
			{
				Handmanager.Handcard hc = p.searchRandomMinionInHand(p.owncards, searchmode.searchLowestCost, GAME_TAGs.None, TAG_RACE.DRAGON);
				hc.addattack += 2;
				hc.addHp += 2;
			}
		}

	}
}