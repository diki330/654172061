namespace HREngine.Bots
{
	class Sim_GIL_637 : SimTemplate //* 凶猛咆哮 Ferocious Howl
	{
        //Draw a card.Gain 1 Armor for each card in your hand.
        //抽一张牌。你每有一张手牌，便获得1点护甲值。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, ownplay);

            int armor = (ownplay) ? p.owncards.Count : p.enemyAnzCards;
            if (ownplay)
            {
                p.minionGetArmor(p.ownHero, armor);
            }
            else
            {
                p.minionGetArmor(p.enemyHero, armor);
            }
        }

    }
}