using System;
using System.Collections.Generic;
using System.Text;


namespace HREngine.Bots
{

    class Sim_DRG_238p5 : SimTemplate //迦拉克隆的智识
    {
        //英雄技能 随机将一张牧师随从牌置入你的手牌

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own, true);
        }
    }
}
