namespace HREngine.Bots
{
	class Sim_DAL_350a : SimTemplate //* 利刺荆棘 Piercing Thorns
	{
		//Deal $2 damage to a minion.
		//对一个随从造成$2点伤害。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
			p.minionGetDamageOrHeal(target, dmg);
		}
	}

}
