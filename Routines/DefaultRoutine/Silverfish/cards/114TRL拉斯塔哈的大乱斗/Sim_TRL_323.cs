namespace HREngine.Bots
{
    class Sim_TRL_323 : SimTemplate //* 烬鳞幼龙 Emberscale Drake
    {
        //<b>Battlecry:</b> If you're holding a Dragon, gain 5 Armor.
        //<b>战吼：</b>如果你的手牌中有龙牌，便获得5点护甲值。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                bool dragonInHand = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
                    {
                        dragonInHand = true;
                        break;
                    }
                }
                if (dragonInHand) p.minionGetArmor(p.ownHero, 5);
            }
            else
            {
                if (p.enemyAnzCards >= 2) p.minionGetArmor(p.enemyHero, 5);
            }
        }

    }
}