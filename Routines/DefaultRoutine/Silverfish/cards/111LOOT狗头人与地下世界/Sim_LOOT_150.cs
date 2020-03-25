using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_150 : SimTemplate//缚雾熊怪
    {

        private CardDB.Card 迷雾元素 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_150t1);

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionTransform(target, 迷雾元素);
        }

    }
}
