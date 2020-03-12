namespace HREngine.Bots
{
	class Sim_DAL_568ts : SimTemplate //* 光铸祝福 Lightforged Blessing
	{
		//Give a friendly minion <b>Lifesteal</b>.
		//使一个友方随从获得<b>吸血</b>。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			target.lifesteal = true;
		}

	}
}