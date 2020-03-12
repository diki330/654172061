using System;
using System.Collections.Generic;
using System.Text;

    namespace HREngine.Bots
    {
        class Sim_DRG_207 : SimTemplate//* …Ó‘®’ŸªΩ’ﬂ Abyssal Summoner
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_207t);//’ŸªΩ…Ó‘®ªŸ√’ﬂ

            public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
            {
                p.callKid(kid, own.zonepos, own.own);
            }
        
        }
    }
    