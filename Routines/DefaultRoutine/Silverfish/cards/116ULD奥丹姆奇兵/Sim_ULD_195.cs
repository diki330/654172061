namespace HREngine.Bots
{
	class Sim_ULD_195 : SimTemplate //* 惊恐的仆从 Frightened Flunky
	{
		//<b>Taunt</b><b>Battlecry:</b> <b>Discover</b> a <b>Taunt</b> minion.
		//<b>嘲讽，战吼：</b><b>发现</b>一张具有<b>嘲讽</b>的随从牌。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.drawACard(CardDB.cardName.unknown, own.own, true);
		}

	}
}