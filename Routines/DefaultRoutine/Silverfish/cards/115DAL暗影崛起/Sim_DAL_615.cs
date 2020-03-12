using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_DAL_615 : SimTemplate
    {
        public List<CardDB.cardIDEnum> minionsToSkip = new List<CardDB.cardIDEnum>()
        {
            CardDB.cardIDEnum.ULD_276,//¹ÖµÁÍ¼ÌÚ
            CardDB.cardIDEnum.OG_271,//ÃÎ÷ÊÖ®Áú
            CardDB.cardIDEnum.NEW1_009//ÖÎÁÆÍ¼ÌÚ
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