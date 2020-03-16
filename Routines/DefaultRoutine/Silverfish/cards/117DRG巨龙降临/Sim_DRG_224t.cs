using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_224t : SimTemplate //·ç±©ÁúÂÑ
    {

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_224t2);

        public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            p.minionTransform(triggerEffectMinion, kid);
        }
    }
}