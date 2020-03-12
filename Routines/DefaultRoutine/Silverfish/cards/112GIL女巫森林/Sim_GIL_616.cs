namespace HREngine.Bots
{
	class Sim_GIL_616 : SimTemplate //* 分裂腐树 Splitting Festeroot
	{
		//<b>Deathrattle:</b> Summon two 2/2 Splitting Saplings.
		//<b>亡语：</b>召唤两个2/2的分裂树苗。
		public override void onDeathrattle(Playfield p, Minion m)
		{
			var card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.GIL_616t);
			p.callKid(card, m.zonepos - 1, m.own, true, true);
			p.callKid(card, m.zonepos - 1, m.own, true, true);
		}

	}
}