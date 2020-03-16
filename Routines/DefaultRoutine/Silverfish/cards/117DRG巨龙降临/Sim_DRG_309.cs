using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_309 : SimTemplate
    {
        //时光巨龙诺兹多姆
        //将双方玩家的法力水晶重置为十个

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.ownMaxMana = 10;
            p.enemyMaxMana = 10;

        }

    }
}