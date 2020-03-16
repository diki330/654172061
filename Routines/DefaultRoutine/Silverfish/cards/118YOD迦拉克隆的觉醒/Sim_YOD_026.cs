using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_YOD_026 : SimTemplate
    {
        //аħ�ʹ�
        //������ʹһ���ѷ���ӻ�ø���ӵĹ�������
        public override void onDeathrattle(Playfield p, Minion m)
        {
            int attack = m.Angr;
            Minion target = (m.own) ? p.searchRandomMinion(p.ownMinions, searchmode.searchLowestAttack) : p.searchRandomMinion(p.enemyMinions, searchmode.searchLowestAttack);
            if (target != null) p.minionGetBuffed(target, attack, 0);
        }

    }
}
