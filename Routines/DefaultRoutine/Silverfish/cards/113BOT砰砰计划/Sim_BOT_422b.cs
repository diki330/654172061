namespace HREngine.Bots
{
	class Sim_BOT_422b : SimTemplate //* 新生幼苗 New Growth
	{
		//Summon two 2/2 Treants.
		//召唤两个2/2的树人。
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_573t); //special treant
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
			p.callKid(kid, pos, ownplay, false);
			p.callKid(kid, pos, ownplay);
		}

	}
}