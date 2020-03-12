namespace HREngine.Bots
{
	class Sim_DRG_304 : SimTemplate //* 时空破坏者 Chronobreaker
	{
		//[x]<b>Deathrattle:</b> If you're holdinga Dragon, deal 3 damageto all enemy minions.
		//<b>亡语：</b>如果你的手牌中有龙牌，则对所有敌方随从造成3点伤害。
		public override void onDeathrattle(Playfield p, Minion m)
		{
			if(m.own)
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
				if(dragonInHand) p.allMinionOfASideGetDamage(!m.own, 3);
			}
			else
			{
				if (p.enemyAnzCards >= 2) p.allMinionOfASideGetDamage(m.own, 3);
			}
		}

	}
}