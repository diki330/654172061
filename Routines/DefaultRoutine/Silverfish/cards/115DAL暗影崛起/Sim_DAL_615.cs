using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_DAL_615 : SimTemplate
    {
        public List<CardDB.cardIDEnum> minionsToSkip = new List<CardDB.cardIDEnum>()
        {
            CardDB.cardIDEnum.ULD_276,//�ֵ�ͼ��
            CardDB.cardIDEnum.OG_271,//����֮��
            CardDB.cardIDEnum.NEW1_009//����ͼ��
        };

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                if (minionsToSkip.Contains(target.handcard.card.cardIDenum))
                {
                    if (target.silenced)
                    {
                        p.minionTransform(target, p.getRandomCardForManaMinion(target.handcard.card.cost + 1));
                    }
                    else
                    {
                        p.evaluatePenality += 100;
                    }
                }
                else
                {
                    p.minionTransform(target, p.getRandomCardForManaMinion(target.handcard.card.cost + 1));
                }
            }
            else
            {
                p.evaluatePenality += 50;
            }
        }
    }
}