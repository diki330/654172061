namespace HREngine.Bots
{
	class Sim_ULD_328 : SimTemplate //* 聪明的伪装 Clever Disguise
	{
		//Add 2 random spells from another class to your hand.
		//随机将另一职业的两张法术牌置入你的手牌。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.drawACard(CardDB.cardName.unknown, ownplay, true);
			p.drawACard(CardDB.cardName.unknown, ownplay, true);
		}

	}
}