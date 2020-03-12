namespace HREngine.Bots
{
	class Sim_TRL_254a : SimTemplate //* 贡克的坚韧 Gonk's Resilience
	{
		//Give a minion +2/+4 and <b>Taunt</b>.
		//使一个随从获得+2/+4和<b>嘲讽</b>。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.minionGetBuffed(target, 2, 2);
			if (!target.taunt)
			{
				target.taunt = true;
				if (target.own) p.anzOwnTaunt++;
				else p.anzEnemyTaunt++;
			}
		}

	}
}