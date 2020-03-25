using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_511 : SimTemplate //* 卡瑟娜冬灵
    {
        //战吼，亡语：</b><b>招募</b>一个野兽

        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.callKid(p.getNextJadeGolem(m.own), m.zonepos, m.own);
        }

        public override void onDeathrattle(Playfield p, Minion m)
        {
            p.callKid(p.getNextJadeGolem(m.own), m.zonepos - 1, m.own);
        }
    }
}