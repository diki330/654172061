namespace HREngine.Bots
{
	class Sim_BOT_517 : SimTemplate //* 引力翻转 Topsy Turvy
	{
		//Swap a minion's Attack and Health.
		//使一个随从的攻击力和生命值互换。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.minionSwapAngrAndHP(target);
		}

	}
}