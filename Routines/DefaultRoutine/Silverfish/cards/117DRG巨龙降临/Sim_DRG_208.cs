using System;
using System.Collections.Generic;
using System.Text;

    namespace HREngine.Bots
    {
        class Sim_DRG_208 : SimTemplate//* �ߵ���˹��а�� Valdris Felgorge
        {
        //<b>ս��</b>������������������12�š��������ơ�

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
            {
                p.drawACard(CardDB.cardIDEnum.None, own.own);//ֻд�˳�������
                p.drawACard(CardDB.cardIDEnum.None, own.own);
                p.drawACard(CardDB.cardIDEnum.None, own.own);
                p.drawACard(CardDB.cardIDEnum.None, own.own);
        }
        
        }
    }
    