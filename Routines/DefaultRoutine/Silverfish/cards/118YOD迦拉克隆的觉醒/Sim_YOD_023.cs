using System;
using System.Collections.Generic;
using System.Text;

    namespace HREngine.Bots
    {
        class Sim_YOD_023 : SimTemplate
        {
        //����ս��
        //����һ�Ÿ����ƣ���е�ƻ����ơ�

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
                {
                    p.drawACard(CardDB.cardName.unknown, ownplay, true);
                }
        }
    }
    