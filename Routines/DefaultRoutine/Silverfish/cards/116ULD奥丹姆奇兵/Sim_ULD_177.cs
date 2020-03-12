namespace HREngine.Bots
{
	class Sim_ULD_177 : SimTemplate //* 八爪巨怪 Octosari
	{
        //<b>Deathrattle:</b> Draw 8 cards.
        //<b>亡语：</b>抽八张牌。
        public override void onDeathrattle(Playfield p, Minion m)
        {
            for (int i = 0; i < 8; i++)
            {
                p.drawACard(CardDB.cardName.unknown, m.own);
            }
        }

    }
}