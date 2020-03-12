namespace HREngine.Bots
{
	class Sim_GIL_902 : SimTemplate //* 刺喉海盗 Cutthroat Buccaneer
	{
        //<b>Combo:</b> Give your weapon +1 Attack.
        //<b>连击：</b>使你的武器获得+1攻击力。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (p.cardsPlayedThisTurn > 0)
            {
                if (own.own)
                {
                    if (p.ownWeapon.Durability >= 1)
                    {
                        p.ownWeapon.Angr += 1;
                        p.minionGetBuffed(p.ownHero, 1, 0);
                    }
                }
                else
                {
                    if (p.enemyWeapon.Durability >= 1)
                    {
                        p.enemyWeapon.Angr += 1;
                        p.minionGetBuffed(p.enemyHero, 1, 0);
                    }
                }
                p.minionGetBuffed(own, 3, 0);
            }
        }

    }
}