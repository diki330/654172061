namespace HREngine.Bots
{
    class Sim_GIL_807 : SimTemplate //* 塑沼者 Bogshaper
    {
        //Whenever you cast a spell, draw a minion from your deck.
        //每当你施放一个法术，从你的牌库中抽一张随从牌。
        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool wasOwnCard, Minion triggerEffectMinion)
        {
            if (wasOwnCard == triggerEffectMinion.own && hc.card.type == CardDB.cardtype.SPELL)
            {
                p.drawACard(CardDB.cardName.unknown, wasOwnCard);
            }
        }

    }
}