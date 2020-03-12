namespace HREngine.Bots
{
	class Sim_DRG_257 : SimTemplate //* 弗瑞兹·光巢 Frizz Kindleroost
	{
		//<b>Battlecry:</b> Reduce the Cost of Dragons in your deck by (2).
		//<b>战吼：</b>使你牌库中龙牌的法力值消耗减少（2）点。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (own.own) p.ownMinionsInDeckCost0 = true;
		}

	}
}