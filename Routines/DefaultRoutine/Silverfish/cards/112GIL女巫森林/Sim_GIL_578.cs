namespace HREngine.Bots
{
    class Sim_GIL_578 : SimTemplate //* 女伯爵阿莎摩尔 Countess Ashmore
    {
        //[x]<b>Battlecry:</b> Draw a <b>Rush</b>,<b>Lifesteal</b>, and <b>Deathrattle</b>card from your deck.
        //<b>战吼：</b>从你的牌库中抽一张<b>突袭</b>牌、<b>吸血</b>牌和<b>亡语</b>牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                if (p.prozis.turnDeck.Count == 0) return;
                foreach (var item in p.prozis.turnDeck)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(item.Key);
                    if (c.Rush)
                    {
                        p.drawACard(c.cardIDenum, own.own);
                        break;
                    }
                }
                foreach (var item in p.prozis.turnDeck)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(item.Key);
                    if (c.lifesteal)
                    {
                        p.drawACard(c.cardIDenum, own.own);
                        break;
                    }
                }
                foreach (var item in p.prozis.turnDeck)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(item.Key);
                    if (c.deathrattle)
                    {
                        p.drawACard(c.cardIDenum, own.own);
                        break;
                    }
                }
            }
        }

    }
}