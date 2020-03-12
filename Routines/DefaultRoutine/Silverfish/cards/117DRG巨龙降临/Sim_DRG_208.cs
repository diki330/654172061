using System;
using System.Collections.Generic;
using System.Text;

    namespace HREngine.Bots
    {
        class Sim_DRG_208 : SimTemplate//* 瓦迪瑞斯·邪噬 Valdris Felgorge
        {
        //<b>战吼：</b>将你的手牌上限提高至12张。抽四张牌。

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
            {
                p.drawACard(CardDB.cardIDEnum.None, own.own);//只写了抽四张牌
                p.drawACard(CardDB.cardIDEnum.None, own.own);
                p.drawACard(CardDB.cardIDEnum.None, own.own);
                p.drawACard(CardDB.cardIDEnum.None, own.own);
        }
        
        }
    }
    