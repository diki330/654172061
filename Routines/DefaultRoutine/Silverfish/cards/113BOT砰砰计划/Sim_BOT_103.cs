namespace HREngine.Bots
{
    class Sim_BOT_103 : SimTemplate //* 观星者露娜 Stargazer Luna
    {
        //After you play theright-most card in your hand, draw a card.
        //在你使用最右边的一张手牌后，抽一张牌。
        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool ownplay, Minion m)
        {
            if (hc.position == (p.owncards.Count - 1) && m.own)
            {
                p.drawACard(CardDB.cardName.unknown, m.own, true);
            }
        }
    }
}