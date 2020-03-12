namespace HREngine.Bots
{
	class Sim_DRG_081 : SimTemplate //* 锐鳞骑士 Scalerider
	{
		//<b>Battlecry:</b> If you're holding a Dragon, deal 2 damage.
		//<b>战吼：</b>如果你的手牌中有龙牌，则造成2点伤害。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
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
			if (dragonInHand) p.minionGetDamageOrHeal(target, 2);
		}

	}
}