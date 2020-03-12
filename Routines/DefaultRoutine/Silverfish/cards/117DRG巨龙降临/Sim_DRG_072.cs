using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_072 : SimTemplate //* 飞天鱼人 Skyfin
	{
		//<b>Battlecry:</b> If you're holding a Dragon, summon 2 random Murlocs.
		//<b>战吼：</b>如果你的手牌中有龙牌，随机召唤两个鱼人。
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_050);
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			bool dragonInHand = false;
			foreach (Handmanager.Handcard hc in p.owncards)
			{
				if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
				{
					dragonInHand = true;
					break;
				}
			}
			if (dragonInHand)
			{
				p.callKid(kid, own.zonepos-1, own.own);
				p.callKid(kid, own.zonepos, own.own);
			}
		}
	}
}