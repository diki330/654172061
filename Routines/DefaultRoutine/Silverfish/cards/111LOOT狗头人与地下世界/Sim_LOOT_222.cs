using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_222 : SimTemplate //蜡烛弓
    {
        CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_222);
        //    euer held ist immun/, während er angreift.
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(card, ownplay);
        }

    }
}