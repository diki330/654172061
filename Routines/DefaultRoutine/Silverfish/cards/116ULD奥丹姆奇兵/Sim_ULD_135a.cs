namespace HREngine.Bots
{
	class Sim_ULD_135a : SimTemplate //* 结识古树 Befriend the Ancient
	{
		//Summon a 6/6 Ancient with <b>Taunt</b>.
		//召唤一棵6/6并具有<b>嘲讽</b>的古树。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			CardDB.Card ancient = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_135at);
			int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
			p.callKid(ancient, pos, ownplay, false);
		}

	}
}