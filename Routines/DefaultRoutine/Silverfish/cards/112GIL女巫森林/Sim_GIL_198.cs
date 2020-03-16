namespace HREngine.Bots
{
    class Sim_GIL_198 : SimTemplate //* 窃魂者阿扎莉娜 Azalina Soulthief
    {
        //<b>Battlecry:</b> Replace your hand with a copy of your opponent's.
        //<b>战吼：</b>将你的手牌替换成对手手牌的复制。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.discardCards(10, own.own);
            for (int i = 0; i < p.enemyAnzCards; i++)
            {
                p.drawACard(CardDB.cardName.unknown, own.own, true);
            }
        }

    }
}