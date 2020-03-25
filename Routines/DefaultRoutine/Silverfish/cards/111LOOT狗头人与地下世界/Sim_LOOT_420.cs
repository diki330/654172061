using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    internal class Sim_LOOT_420 : SimTemplate //堕落者之颅
    {
        //   在你的回合开始时，从你的手牌中召唤一个恶魔.
        private CardDB.Card w = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_420);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(w, ownplay);

        }
    }
}