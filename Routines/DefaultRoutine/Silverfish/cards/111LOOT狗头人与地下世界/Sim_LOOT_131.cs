using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_131 : SimTemplate //绿色凝胶怪
    {
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_131t1);//绿色软泥怪
                                                                                          //    ruft am ende eures zuges einen gnoll (1/2) mit spott/ herbei.

        public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            if (triggerEffectMinion.own == turnEndOfOwner)
            {
                int posi = triggerEffectMinion.zonepos;

                p.callKid(kid, posi, triggerEffectMinion.own);
            }
        }

    }
}