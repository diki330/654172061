namespace HREngine.Bots
{
	class Sim_DRG_063 : SimTemplate //* 龙喉偷猎者 Dragonmaw Poacher
	{
		//<b>Battlecry:</b> If your opponent controls a Dragon, gain +4/+4 and <b>Rush</b>.
		//<b>战吼：</b>如果你的对手控制着一条龙，便获得+4/+4和<b>突袭</b>。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			bool Havedragon = false;
			foreach (Minion m in p.enemyMinions)
			{
				if ((TAG_RACE)m.handcard.card.race == TAG_RACE.DRAGON)
				{
					Havedragon = true;
					break;
				}
			}
			if (Havedragon)
			{
				p.minionGetBuffed(own, 4, 4);
				p.minionGetRush(own);
			}	
				
		}

	}
}