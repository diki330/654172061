namespace HREngine.Bots
{
	class Sim_LOOT_998j : SimTemplate //* 扎罗格的王冠 Zarog's Crown
	{
		//<b>Discover</b> a <b>Legendary</b> minion. Summon two copies of it.
		//<b>发现</b>一张<b>传说</b>随从牌。召唤两个它的复制。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
			if (p.ownHeroHasDirectLethal()) p.callKid(CardDB.Instance.getCardData(CardDB.cardName.icehowl), pos, ownplay, false);
			else
			{
				p.callKid(CardDB.Instance.getCardData(CardDB.cardName.frostgiant), pos, ownplay, false);
				p.callKid(CardDB.Instance.getCardData(CardDB.cardName.frostgiant), pos, ownplay, true);
			}
		}

	}
}