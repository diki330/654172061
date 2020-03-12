namespace HREngine.Bots
{
	class Sim_DRG_307 : SimTemplate //* 永恒吐息 Breath of the Infinite
	{
		//Deal $2 damage to all minions. If you're holding a Dragon, only damage enemies.
		//对所有随从造成$2点伤害。如果你的手牌中有龙牌，则只对敌方随从造成伤害。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
			
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
                     p.allMinionOfASideGetDamage(!ownplay, dmg);
                }
				else
				{
					p.allMinionsGetDamage(dmg);
				}
			}
        }

	}
}