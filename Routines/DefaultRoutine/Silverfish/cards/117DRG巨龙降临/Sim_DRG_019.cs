using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_DRG_019 : SimTemplate //* 废墟之子 Scion of Ruin
    {
        //<b>Rush</b>. <b>Battlecry:</b> If you've <b>Invoked</b> twice, summon 2 copies of this.
        //<b>突袭</b><b>战吼：</b>如果你已经<b>祈求</b>过两次，召唤该随从的两个复制。
        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (p.OwnInvoke >= 2)
            {
                p.callKid(m.handcard.card, m.zonepos, m.own);
                p.callKid(m.handcard.card, m.zonepos, m.own);
                int count = 0;
                foreach (Minion mnn in p.ownMinions)
                {
                    if (mnn.name == CardDB.cardName.scionofruin && m.entitiyID != mnn.entitiyID && mnn.playedThisTurn)
                    {
                        mnn.setMinionToMinion(m);
                        count++;
                        if (count >= 2) break;
                    }
                }
            }
        }

    }
}