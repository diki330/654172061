using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_353 : SimTemplate //* 灵能窥探
    {
        // 复制对手牌库中的一张法术牌,并将其置入你的手牌.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay) p.enemyDeckSize = Math.Max(0, p.enemyDeckSize - 1);
            {
                p.drawACard(CardDB.cardName.unknown, ownplay, true);
            }
        }
    }
}