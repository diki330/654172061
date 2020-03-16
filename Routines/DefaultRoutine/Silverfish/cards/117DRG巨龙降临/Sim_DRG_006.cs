namespace HREngine.Bots
{
    class Sim_DRG_006 : SimTemplate //* 腐蚀吐息 Corrosive Breath
    {
        //[x]Deal $3 damage to aminion. If you're holdinga Dragon, it also hitsthe enemy hero.
        //对一个随从造成$3点伤害。如果你的手牌中有龙牌，还会命中敌方英雄。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
            int dmg2 = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
            p.minionGetDamageOrHeal(target, dmg);
            if (ownplay == true)
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
                if (dragonInHand) p.minionGetDamageOrHeal(p.enemyHero, dmg2);
            }
            else
            {
                if (p.enemyAnzCards >= 2) p.minionGetDamageOrHeal(p.ownHero, dmg2);
            }
        }
    }
}