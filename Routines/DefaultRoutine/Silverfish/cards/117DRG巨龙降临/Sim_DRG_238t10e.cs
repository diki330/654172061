using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_238t10e : SimTemplate //迦拉克隆的巨力
    {

        //    英雄技能 在本回合中，获得+3攻击力.

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                p.minionGetTempBuff(p.ownHero, 3, 0);
            }
            else
            {
                p.minionGetTempBuff(p.enemyHero, 3, 0);
            }
        }

	}
}