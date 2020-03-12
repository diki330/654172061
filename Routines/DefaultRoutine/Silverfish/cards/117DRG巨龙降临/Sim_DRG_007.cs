namespace HREngine.Bots
{
	class Sim_DRG_007 : SimTemplate //* 风暴之锤 Stormhammer
	{
		//Doesn't lose Durability while you control a Dragon.
		//当你控制着一条龙时，不会失去耐久度。
		CardDB.Card wcard = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_007);
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.equipWeapon(wcard, ownplay);
		}

	}
}