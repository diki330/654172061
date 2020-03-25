using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_103 : SimTemplate //* 小型法术红宝石
    {
        //Add 1 random Mage spells to your hand.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.frostbolt, ownplay, true);
        }
    }
}