namespace HREngine.Bots
{
	class Sim_TRL_509 : SimTemplate //* 香蕉小丑 Banana Buffoon
	{
        //<b>Battlecry:</b> Add 2 Bananas to your hand.
        //<b>战吼：</b>将两个香蕉置入你的手牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            int count = 2;
            for (int i = 1; i <= count; i++)
            {
                p.drawACard(CardDB.cardName.bananas, own.own, true);
                //the card draw counter will plus one in drawACard method
                //but the card is not drawn from deck, so we subtract it back
                if (own.own)
                {
                    p.owncarddraw--;
                }
                else
                {
                    p.enemycarddraw--;
                }
            }
        }

    }
}