namespace HREngine.Bots
{
	class Sim_ULD_135b : SimTemplate //* 饮用泉水 Drink the Water
	{
		//Restore #12 Health.
		//恢复#12点生命值。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int heal = (ownplay) ? p.getSpellHeal(12) : p.getEnemySpellHeal(12);
			p.minionGetDamageOrHeal(target, -heal);
		}

	}
}