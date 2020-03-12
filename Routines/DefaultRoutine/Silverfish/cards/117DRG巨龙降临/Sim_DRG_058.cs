namespace HREngine.Bots
{
	class Sim_DRG_058 : SimTemplate //* 空军指挥官 Wing Commander
	{
		//Has +2 Attack for each Dragon in your hand.
		//你手牌中每有一张龙牌，便获得+2攻击力。
		
		public override void onAuraStarts(Playfield p, Minion m)
		{
			if(m.own)
			{
				int Numofdragons = 0;
				foreach (Handmanager.Handcard hc in p.owncards)
				{
					if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
					{
						Numofdragons ++;
						break;
					}
				}
				p.minionGetBuffed(m,2*Numofdragons,0);
			}			
		}
		public override void onAuraEnds(Playfield p, Minion m)
		{
			if (m.own)
			{
				int Numofdragons = 0;
				foreach (Handmanager.Handcard hc in p.owncards)
				{
					if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
					{
						Numofdragons ++;
						break;
					}
				}
				p.minionGetBuffed(m, -2*Numofdragons, 0);
			}
		}
		
	}
}