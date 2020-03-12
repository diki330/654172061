namespace HREngine.Bots
{
	class Sim_GIL_645 : SimTemplate //* 篝火元素 Bonfire Elemental
	{
		//<b>Battlecry:</b> If you played an Elemental last turn, draw a card.
		//<b>战吼：</b>如果你在上个回合使用过元素牌，抽一张牌。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			if (p.anzOwnElementalsLastTurn > 0 && own.own) p.drawACard(CardDB.cardName.unknown, own.own);
		}

	}
}