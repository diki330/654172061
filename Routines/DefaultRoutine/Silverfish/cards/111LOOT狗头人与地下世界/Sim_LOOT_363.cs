using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_363 : SimTemplate //* 旱谷狱卒
    {
        //亡语: 将三个白银之手新兵置入你的手牌.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);//silverhandrecruit

        public override void onDeathrattle(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.CS2_101t, m.own, true);
            p.drawACard(CardDB.cardIDEnum.CS2_101t, m.own, true);
            p.drawACard(CardDB.cardIDEnum.CS2_101t, m.own, true);
        }
    }
}