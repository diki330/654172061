using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots {
    class Sim_DRG_311a : SimTemplate {
        //召唤一个2/2的树人
        CardDB.Card kid = CardDB.Instance.getCardDataFromID (CardDB.cardIDEnum.DRG_311t); //panther

        public override void onCardPlay (Playfield p, bool ownplay, Minion target, int choice) {
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid (kid, pos, ownplay, false);
        }

    }
}