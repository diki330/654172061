using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_211 : SimTemplate //* 精灵咏唱者
    {
        //Combo: 从你的牌库中抽两张随从牌.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (p.cardsPlayedThisTurn >= 1 && target != null)
            {
                p.drawACard(CardDB.cardName.unknown, own.own);
                p.drawACard(CardDB.cardName.unknown, own.own);
            }
        }
    }
}