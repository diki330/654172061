namespace HREngine.Bots
{
    class Sim_ULD_726 : SimTemplate //* 远古谜团 Ancient Mysteries
    {
        //Draw a <b>Secret</b> from your deck. It costs (0).
        //从你的牌库中抽一张<b>奥秘</b>牌。其法力值消耗为（0）点。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                var deck = p.prozis.turnDeck;
                foreach (var item in deck)
                {
                    var card = CardDB.Instance.getCardDataFromID(item.Key);
                    if (card.Secret)
                    {
                        p.drawACard(card.name, ownplay);
                        p.owncards[p.owncards.Count - 1].manacost = 0;
                        break;
                    }
                }
            }
            else
            {
                p.drawACard(CardDB.cardName.unknown, ownplay);
            }

        }
    }
}