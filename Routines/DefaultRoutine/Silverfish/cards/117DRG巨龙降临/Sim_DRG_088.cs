using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_DRG_088 : SimTemplate //* 恐惧渡鸦 Dread Raven
    {
        //Has +3 Attack for each other Dread Raven you control.
        //你每控制一只其他恐惧渡鸦，便获得+3攻击力。
        public override void onAuraStarts(Playfield p, Minion m)
        {
            int bonusattack = 0;
            List<Minion> temp = (m.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion mn in temp)
            {
                if (mn.entitiyID == m.entitiyID) continue;
                if (mn.name == CardDB.cardName.dreadraven) bonusattack++;
            }
            p.minionGetBuffed(m, 3 * bonusattack, 0);
        }
        public override void onAuraEnds(Playfield p, Minion m)
        {
            int bonusattack = 0;
            List<Minion> temp = (m.own) ? p.ownMinions : p.enemyMinions;
            foreach (Minion mn in temp)
            {
                if (mn.entitiyID == m.entitiyID) continue;
                if (mn.name == CardDB.cardName.dreadraven) bonusattack++;
            }
            p.minionGetBuffed(m, -3 * bonusattack, 0);
        }

    }
}