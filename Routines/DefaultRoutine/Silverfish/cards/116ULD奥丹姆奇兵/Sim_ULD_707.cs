namespace HREngine.Bots
{
	class Sim_ULD_707 : SimTemplate //* 愤怒之灾祸 Plague of Wrath
	{
        //Destroy all damaged minions.
        //消灭所有受伤的随从。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            foreach (Minion m in p.ownMinions)
            {
                if (m.wounded) p.minionGetDestroyed(m);
            }
            foreach (Minion m in p.enemyMinions)
            {
                if (m.wounded) p.minionGetDestroyed(m);
            }
        }

    }
}