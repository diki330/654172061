using System;

namespace HREngine.Bots
{
	class Sim_ULD_717 : SimTemplate //* 火焰之灾祸 Plague of Flames
	{
        //[x]Destroy all your minions.For each one, destroy arandom enemy minion.
        //消灭你的所有随从。每消灭一个随从，便随机消灭一个敌方随从。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int destroyMinionNum = Math.Min(p.ownMinions.Count, p.enemyMinions.Count);
            foreach (Minion m in p.ownMinions)
            {
                p.minionGetDestroyed(m);
            }
            for (int i = 0; i < destroyMinionNum; i++)
            {
                Minion m = p.searchRandomMinion(ownplay ? p.enemyMinions : p.ownMinions, searchmode.searchLowestHP);
                if (m != null) p.minionGetDestroyed(m);
            }
        }

    }
}