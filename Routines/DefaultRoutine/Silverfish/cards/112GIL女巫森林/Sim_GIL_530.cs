using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{

    public class Sim_GIL_530 : SimTemplate //��ȼ����
    {
        //<b>ս��</b>�������ƿ���ֻ�з���ֵ����Ϊż�����ƣ����2���˺���
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                int damage = 2;
                p.minionGetDamageOrHeal(target, damage);
                
            }
        }
    }

}