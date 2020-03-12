namespace HREngine.Bots
{
	class Sim_ULD_250 : SimTemplate //* 招虫的地精 Infested Goblin
	{
		//<b>Taunt</b><b>Deathrattle:</b> Add two 1/1 Scarabs with <b>Taunt</b> to your hand.
		//<b>嘲讽，亡语：</b>将两张1/1并具有<b>嘲讽</b>的“甲虫”置入你的手牌。
		public override void onDeathrattle(Playfield p, Minion m)
		{
			CardDB.Card scarab = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_215t);//甲虫 Scarab

			p.drawACard(scarab.name, m.own, true);
			p.drawACard(scarab.name, m.own, true);
		}

	}
}