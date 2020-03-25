using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_170 : SimTemplate //* 乌鸦魔仆
    {
        //战吼: 揭示双方牌库里的一张法术牌. If yours costs more, draw it.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.fireball, own.own);
        }
    }
}