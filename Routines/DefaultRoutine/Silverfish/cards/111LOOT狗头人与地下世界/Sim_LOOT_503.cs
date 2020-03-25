using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_503 : SimTemplate //* 小型法术黑曜石
    {
        // Destroy a random enemy minion.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            Minion m = p.searchRandomMinion(ownplay ? p.enemyMinions : p.ownMinions, searchmode.searchLowestHP);
            if (m != null) p.minionGetDestroyed(m);
        }

    }
}