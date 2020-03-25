using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_389 : SimTemplate //* 狗头人拾荒者
    {
        //Battlecry: 将你的一把被摧毁的武器置入你的手牌.

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.fierywaraxe, own.own, true);
        }
    }
}