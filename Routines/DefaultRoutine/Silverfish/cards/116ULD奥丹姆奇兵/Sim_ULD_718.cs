namespace HREngine.Bots
{
	class Sim_ULD_718 : SimTemplate //* 死亡之灾祸 Plague of Death
	{
		//<b>Silence</b> and destroy all minions.
		//<b>沉默</b>并消灭所有随从。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.allMinionsGetSilenced(ownplay);
			p.allMinionsGetSilenced(!ownplay);
			p.allMinionsGetDestroyed();
		}

	}
}