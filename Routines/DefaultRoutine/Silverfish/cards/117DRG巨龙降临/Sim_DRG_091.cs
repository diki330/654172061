using System;
using System.Collections.Generic;
using System.Text;


namespace HREngine.Bots
{
    class Sim_DRG_091 : SimTemplate //* 舒玛 Shu'ma
    {
        //At the end of your turn,fill your board with 1/1 Tentacles.
        //在你的回合结束时，召唤数条1/1的触手，直到你的随从数量达到上限。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_091t);//Tentacle

        //    kampfschrei:/ ruft welplinge (1/1) herbei, bis eure seite des schlachtfelds voll ist.
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            int kids = 7 - p.ownMinions.Count;

            for (int i = 0; i < kids; i++)
            {
                p.callKid(kid, own.zonepos, own.own);
            }

        }


    }
}