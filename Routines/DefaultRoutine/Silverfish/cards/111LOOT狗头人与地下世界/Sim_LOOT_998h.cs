namespace HREngine.Bots
{
    class Sim_LOOT_998h : SimTemplate //* 托林的酒杯 Tolin's Goblet
    {
        //Draw a card. Fill your hand with copies of it.
        //抽一张牌。将该牌的复制置入你的手牌，直到你的手牌数量达到上限。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, ownplay);
            if (ownplay) p.owncarddraw += 10 - p.owncards.Count;
        }

    }
}