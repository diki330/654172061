using System.Collections.Generic;

namespace HREngine.Bots
{
	class Sim_ULD_155p : SimTemplate //* 拉穆卡恒的咆哮 Ramkahen Roar
	{
        //<b>Hero Power</b>Give your minions +2 Attack.
        //<b>英雄技能</b>使你的所有随从获得+2攻击力。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;
            foreach (Minion m in temp)
            {
                p.minionGetBuffed(m, 2, 0);
            }
        }

    }
}