namespace HREngine.Bots
{
	class Sim_ULD_190 : SimTemplate //* 深坑鳄鱼 Pit Crocolisk
	{
		//<b>Battlecry:</b> Deal 5 damage.
		//<b>战吼：</b>造成5点伤害。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.minionGetDamageOrHeal(target, 5);
		}

	}
}