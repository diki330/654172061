using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_TRL_065 : SimTemplate //* 祖尔金 Zul'jin
    {
        //[x]<b>Battlecry:</b> Cast all spellsyou've played this game<i>(targets chosen randomly)</i>.
        //<b>战吼：</b>施放你在本局对战中使用过的所有法术<i>（目标随机而定）</i>。
        CardDB cdb = CardDB.Instance;
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_077t);//Wolf
        CardDB.Card kid2 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DAL_378t1);//双足飞龙
        CardDB.Card kid3 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NEW1_032);//misha


        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {

            p.setNewHeroPower(CardDB.cardIDEnum.TRL_065h, ownplay); //
            if (ownplay) p.ownHero.armor += 5;
            else p.enemyHero.armor += 5;


            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            if (ownplay)
            {


                foreach (KeyValuePair<CardDB.cardIDEnum, int> e in Probabilitymaker.Instance.ownCardsOut)
                {
                    kid = cdb.getCardDataFromID(e.Key);

                    if (kid.type == CardDB.cardtype.SPELL)
                    {
                        {


                            if (kid.Secret)
                            {
                                if (p.ownSecretsIDList.Count < 5 && !p.ownSecretsIDList.Contains(kid.cardIDenum))
                                    p.ownSecretsIDList.Add(kid.cardIDenum);

                            }
                            else if (kid.name == CardDB.cardName.猛兽出笼)
                            {
                                p.callKid(kid2, pos, ownplay);
                                p.drawACard(CardDB.cardName.unknown, ownplay, true);
                                if (e.Value > 1) p.callKid(kid2, pos, ownplay);
                            }
                            else if (kid.name == CardDB.cardName.animalcompanion)
                            {
                                if (e.Value > 1) p.callKid(kid3, pos, ownplay);
                                p.callKid(kid3, pos, ownplay);
                            }
                            else if (kid.name == CardDB.cardName.主人的召唤)
                            {
                                p.drawACard(CardDB.cardName.unknown, ownplay, true);
                                p.drawACard(CardDB.cardName.unknown, ownplay, true);
                                if (e.Value > 1)
                                {
                                    p.drawACard(CardDB.cardName.unknown, ownplay, true);
                                    p.drawACard(CardDB.cardName.unknown, ownplay, true);
                                }
                                p.drawACard(CardDB.cardName.unknown, ownplay, true);
                                if (e.Value > 1)
                                {
                                    p.drawACard(CardDB.cardName.unknown, ownplay, true);
                                }
                            }
                        }


                    }
                }
            }

        }

    }
}