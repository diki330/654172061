using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_204 : SimTemplate //* 诈死
    {
        // Secret: When a friendly minion dies, return it to your hand.

        public override void onSecretPlay(Playfield p, bool ownplay, int number)
        {
            if (ownplay)
            {
                p.drawACard(p.revivingOwnMinion, ownplay, true);
            }
            else
            {
                p.drawACard(p.revivingEnemyMinion, ownplay, true);
            }

        }
    }
}