using System;
using System.Collections.Generic;
using System.Text;

    namespace HREngine.Bots
    {
        class Sim_DRG_207 : SimTemplate//* ��Ԩ�ٻ��� Abyssal Summoner
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_207t);//�ٻ���Ԩ������

            public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
            {
                p.callKid(kid, own.zonepos, own.own);
            }
        
        }
    }
    