namespace HREngine.Bots
{
	class Sim_DRG_084 : SimTemplate //* 触手恐吓者 Tentacled Menace
	{
		//<b>Battlecry:</b> Each player draws a card. Swap their Costs.
		//<b>战吼：</b>每个玩家抽一张牌，交换其法力值消耗。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.drawACard(CardDB.cardName.unknown, own.own);
			p.drawACard(CardDB.cardName.unknown, !own.own);
		}

	}
}