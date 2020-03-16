namespace HREngine.Bots
{
    class Sim_DRG_036 : SimTemplate //* 蜡烛巨龙 Waxadred
    {
        //[x]<b>Deathrattle:</b> Shuffle aCandle into your deck thatresummons Waxadredwhen drawn.
        //<b>亡语：</b>将一支蜡烛洗入你的牌库。抽到蜡烛时，重新召唤蜡烛巨龙。
        public override void onDeathrattle(Playfield p, Minion m)
        {

            if (m.own)
            {
                p.prozis.turnDeck.Add(CardDB.cardIDEnum.DRG_036t, 1);
                p.ownDeckSize++;
            }
            else p.enemyDeckSize++;
        }

    }
}