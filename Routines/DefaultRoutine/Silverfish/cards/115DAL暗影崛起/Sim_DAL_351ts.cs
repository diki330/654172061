namespace HREngine.Bots
{
	class Sim_DAL_351ts : SimTemplate //* 远古祝福 Blessing of the Ancients
	{
		//Give your minions +1/+1.
		//使你的所有随从获得+1/+1。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.allMinionOfASideGetBuffed(ownplay, 1, 1);
		}
	}
}