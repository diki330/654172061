namespace HREngine.Bots
{
	class Sim_BOT_246 : SimTemplate //* 瓶装闪电 Beakered Lightning
	{
		//Deal $1 damage to all minions. <b>Overload:</b> (2)
		//对所有随从造成$1点伤害。<b>过载：</b>（2）
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
			p.allMinionsGetDamage(dmg);
			if (ownplay) p.ueberladung += 2;
		}

	}
}