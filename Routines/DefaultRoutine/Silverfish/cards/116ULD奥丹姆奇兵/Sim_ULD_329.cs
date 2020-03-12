namespace HREngine.Bots
{
	class Sim_ULD_329 : SimTemplate //* 沙丘塑形师 Dune Sculptor
	{
        //[x]After you cast a spell,add a random Mageminion to your hand.
        //在你施放一个法术后，随机将一张法师随从牌置入你的手牌。
        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool wasOwnCard, Minion triggerEffectMinion)
        {
            if (wasOwnCard == triggerEffectMinion.own && hc.card.type == CardDB.cardtype.SPELL)
            {
                p.drawACard(CardDB.cardName.unknown, wasOwnCard, true);
            }
        }

    }
}