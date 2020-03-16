using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_620 : SimTemplate //* 风暴巨龙迦拉克隆 Galakrond, the Tempest
    {
        //<b>Battlecry:</b> Summon two (2/2) Storms with <b>Rush</b>.
        //<b>战吼：</b>召唤两个2/2并具有<b>突袭</b>的风暴。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_620t4);
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
                p.callKid(kid, own.zonepos - 1, own.own);
            p.callKid(kid, own.zonepos, own.own);
        }
    }
}