using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_213 : SimTemplate //* 双头暴虐龙 Twin Tyrant
	{
        //<b>Battlecry:</b> Deal 4 damage to two random enemy minions.
        //<b>战吼：</b>随机对两个敌方随从造成4点伤害。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> tmp = own.own ? p.enemyMinions:p.ownMinions;
            Minion m1 = p.searchRandomMinion(tmp, searchmode.searchLowestHP);
            Minion m2 = p.searchRandomMinion(tmp, searchmode.searchHighestHP);
            p.minionGetDamageOrHeal(m1, 4);
            p.minionGetDamageOrHeal(m2, 4);
        }  

    }
}