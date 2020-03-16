using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_238p4 : SimTemplate //* 迦拉克隆的愤怒
    {
        //英雄技能 召唤一个2/1并具有突袭的元素.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_238t14t3);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int place = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, place, ownplay, false);
            p.callKid(kid, place, ownplay);
        }
    }
}