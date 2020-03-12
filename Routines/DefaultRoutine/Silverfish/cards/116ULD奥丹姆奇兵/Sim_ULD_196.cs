using System.Collections.Generic;
namespace HREngine.Bots
{
	class Sim_ULD_196 : SimTemplate //* 尼斐塞特仪祭师 Neferset Ritualist
	{
        //<b>Battlecry:</b> Restore adjacent minions to full Health.
        //<b>战吼：</b>为相邻的随从恢复所有生命值。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion mnn in temp)
            {
                if (mnn.zonepos == own.zonepos - 1 || mnn.zonepos == own.zonepos + 1)
                {
                    int heal = (own.own) ? p.getMinionHeal(mnn.maxHp - mnn.Hp) : p.getEnemyMinionHeal(mnn.maxHp - mnn.Hp);
                    p.minionGetDamageOrHeal(mnn, -heal, true);
                }
            }
        }

    }
}