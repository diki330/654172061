using System;
using System.Collections.Generic;
using System.Text;


namespace HREngine.Bots
{

    class Sim_DRG_238p5 : SimTemplate //������¡����ʶ
    {
        //Ӣ�ۼ��� �����һ����ʦ����������������

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own, true);
        }
    }
}
