using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_YOD_022 : SimTemplate
    {
        //ð����ͧ��
        //����ʹ��һ������ƺ󣬶�����������1���˺���

        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool ownplay, Minion m)
        {
            if (m.own == ownplay && hc.card.type == CardDB.cardtype.MOB) p.allMinionsGetDamage(1);
        }

    }
}
