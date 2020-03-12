namespace HREngine.Bots
{
	class Sim_ULD_266 : SimTemplate //* 木奶伊 Grandmummy
	{
		//[x]<b>Reborn</b> <b>Deathrattle:</b> Give a randomfriendly minion +1/+1.
		//<b>复生，亡语：</b>随机使一个友方随从获得+1/+1。
		public override void onDeathrattle(Playfield p, Minion m)
		{
			Minion target = (m.own) ? p.searchRandomMinion(p.ownMinions, searchmode.searchLowestAttack) : p.searchRandomMinion(p.enemyMinions, searchmode.searchLowestAttack);
			if (target != null) p.minionGetBuffed(target, 1, 1);
		}

	}
}