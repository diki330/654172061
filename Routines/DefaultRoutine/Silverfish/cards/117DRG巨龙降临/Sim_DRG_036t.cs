namespace HREngine.Bots
{
	class Sim_DRG_036t : SimTemplate //* 巨龙的蜡烛 Waxadred's Candle
	{
		//<b>Casts When Drawn</b>Summon Waxadred.
		//<b>抽到时施放</b>召唤蜡烛巨龙。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			//这个应该不用写sim
			p.callKid(CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_036), ownplay ? p.ownMinions.Count : p.enemyMinions.Count, ownplay);
		
		}

	}
}