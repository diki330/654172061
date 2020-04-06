using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_225 : SimTemplate //���з�צ
    {

        //    ���������е���+1��������ս���ٻ�����1/1��΢���������
        public override void onAuraStarts(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnRaidleader++;
                foreach (Minion m in p.ownMinions)
                {
                    if (own.entitiyID != m.entitiyID && (TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) p.minionGetBuffed(m, 1, 0);
                }
            }
            else
            {
                p.anzEnemyRaidleader++;
                foreach (Minion m in p.enemyMinions)
                {
                    if (own.entitiyID != m.entitiyID && (TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) p.minionGetBuffed(m, 1, 0);
                }
            }

        }

        public override void onAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnRaidleader--;
                foreach (Minion m in p.ownMinions)
                {
                    if (own.entitiyID != m.entitiyID && (TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) p.minionGetBuffed(m, -1, 0);
                }
            }
            else
            {
                p.anzEnemyRaidleader--;
                foreach (Minion m in p.enemyMinions)
                {
                    if (own.entitiyID != m.entitiyID && (TAG_RACE)m.handcard.card.race == TAG_RACE.MECHANICAL) p.minionGetBuffed(m, -1, 0);
                }
            }
        }

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_225t); //1/1 Skeleton

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(kid, own.zonepos - 1, own.own); //1st left
            p.callKid(kid, own.zonepos, own.own);
        }

    }
}