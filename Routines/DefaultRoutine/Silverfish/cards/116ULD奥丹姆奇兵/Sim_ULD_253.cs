namespace HREngine.Bots
{
	class Sim_ULD_253 : SimTemplate //* 陵墓守望者 Tomb Warden
	{
		//<b>Taunt</b><b>Battlecry:</b> Summon a copy of this minion.
		//<b>嘲讽</b><b>战吼：</b>召唤一个该随从的复制。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.callKid(own.handcard.card, own.zonepos, own.own);
			p.ownMinions[own.zonepos + 1].setMinionToMinion(own);
		}

	}
}