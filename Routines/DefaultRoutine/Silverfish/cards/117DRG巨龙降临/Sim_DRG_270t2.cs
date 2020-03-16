using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_270t2 : SimTemplate //* 玛里苟斯的智慧秘典(Malygos's Tome)
    {
        // [x]Add 3 random Mage spells to your hand.
        // 随机将三张法师法术牌置入你的手牌。 	
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.frostbolt, ownplay, true);
            p.drawACard(CardDB.cardName.frostnova, ownplay, true);
            p.drawACard(CardDB.cardName.frostnova, ownplay, true);
        }
    }


}