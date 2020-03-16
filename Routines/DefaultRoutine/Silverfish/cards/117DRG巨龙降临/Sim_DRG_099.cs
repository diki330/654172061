namespace HREngine.Bots
{
    class Sim_DRG_099 : SimTemplate //* 克罗斯·龙蹄 Kronx Dragonhoof
    {
        //[x]<b>Battlecry:</b> Draw Galakrond.If you're already Galakrond,unleash a Devastation.
        //<b>战吼：</b>抽取迦拉克隆。如果你已经变为迦拉克隆，则释放一场灾难。       
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_099t2t);
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            if (p.ownHeroAblility.card.cardIDenum == CardDB.cardIDEnum.DRG_238p5 ||
                p.ownHeroAblility.card.cardIDenum == CardDB.cardIDEnum.DRG_238p4 ||
                p.ownHeroAblility.card.cardIDenum == CardDB.cardIDEnum.DRG_238p3 ||
                p.ownHeroAblility.card.cardIDenum == CardDB.cardIDEnum.DRG_238p2 ||
                p.ownHeroAblility.card.cardIDenum == CardDB.cardIDEnum.DRG_238p)
            {

                if (choice == 4)
                {
                    p.allMinionsGetDamage(5, own.entitiyID);
                }

                if (choice == 3)
                {
                    p.allMinionOfASideGetBuffed(own.own, 2, 2);
                }
                if (choice == 2)
                {
                    int posi = own.own ? p.ownMinions.Count : p.enemyMinions.Count;
                    p.callKid(kid, posi, own.own, false);
                }
                if (choice == 1)
                {
                    p.minionGetDamageOrHeal(p.enemyHero, 5);
                    p.minionGetDamageOrHeal(p.ownHero, -5);

                }
            }
            else if (own.own)
            {
                switch (p.ownHeroStartClass)
                {
                    case TAG_CLASS.MAGE:

                        break;
                    case TAG_CLASS.HUNTER:

                        break;
                    case TAG_CLASS.PRIEST:
                        p.drawACard(CardDB.cardIDEnum.DRG_660, own.own, false);
                        break;
                    case TAG_CLASS.SHAMAN:
                        p.drawACard(CardDB.cardName.风暴巨龙迦拉克隆, own.own, false);//620
                        break;
                    case TAG_CLASS.PALADIN:

                        break;
                    case TAG_CLASS.DRUID:

                        break;
                    case TAG_CLASS.WARLOCK:
                        p.drawACard(CardDB.cardIDEnum.DRG_600, own.own, false);
                        break;
                    case TAG_CLASS.WARRIOR:
                        p.drawACard(CardDB.cardIDEnum.DRG_650, own.own, false);
                        break;
                    case TAG_CLASS.ROGUE:
                        p.drawACard(CardDB.cardIDEnum.DRG_610, own.own, false);
                        break;
                }
            }
        }

    }
}