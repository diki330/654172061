using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_620t2 : SimTemplate //* 天降浩劫迦拉克隆 Galakrond, the Apocalypse
	{
		//<b>Battlecry:</b> Summon two (4/4) Storms with <b>Rush</b>.
		//<b>战吼：</b>召唤两个4/4并具有<b>突袭</b>的风暴。
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_620t5);
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (own.own)
			p.callKid(kid, own.zonepos - 1, own.own);
			p.callKid(kid, own.zonepos, own.own);
		}
	}
}