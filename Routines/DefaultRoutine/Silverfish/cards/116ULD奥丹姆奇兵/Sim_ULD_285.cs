namespace HREngine.Bots
{
	class Sim_ULD_285 : SimTemplate //* 钩镰弯刀 Hooked Scimitar
	{
        //[x]<b>Combo:</b> Gain +2 Attack.
        //<b>连击：</b>获得+2攻击力。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_285);
            p.equipWeapon(weapon, ownplay);
            if (ownplay)
            {
                if (p.cardsPlayedThisTurn >= 1)
                {
                    p.ownWeapon.Angr += 2;
                    p.minionGetBuffed(p.ownHero, 2, 0);
                }
            }
            else
            {
                if (p.cardsPlayedThisTurn >= 1)
                {
                    p.enemyWeapon.Angr += 2;
                    p.minionGetBuffed(p.enemyHero, 2, 0);
                }
            }
        }

    }
}