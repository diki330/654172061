namespace HREngine.Bots
{
	class Sim_DRG_500 : SimTemplate //* 熔火吐息 Molten Breath
	{
		//[x]Deal $5 damage to aminion. If you're holdinga Dragon, gain 5 Armor.
		//对一个随从造成$5点伤害。如果你的手牌中有龙牌，便获得5点护甲值。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int damage = (ownplay) ? p.getSpellDamageDamage(5) : p.getEnemySpellDamageDamage(5);
			p.minionGetDamageOrHeal(target, damage);
            if (ownplay)
            {
                bool dragonInHand = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
                    {
                        dragonInHand = true;
                        break;
                    }
                }
                if (dragonInHand)
                {
                    p.minionGetArmor(p.ownHero, 5);
                }
			}
        }

	}
}