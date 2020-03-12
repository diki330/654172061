namespace HREngine.Bots
{
	class Sim_LOOT_101 : SimTemplate //* 爆炸符文 Explosive Runes
	{
        //<b>Secret:</b> After your opponent plays a minion, deal $6 damage to it and any excess to their hero.
        //<b>奥秘：</b>在你的对手使用一张随从牌后，对该随从造成$6点伤害，超过其生命值上限的伤害将由对方英雄承受。
        public override void onSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
            int dmg2minion = (ownplay) ? p.getSpellDamageDamage(6) : p.getEnemySpellDamageDamage(6);
            int damg2hero = 0;
            p.minionGetDamageOrHeal(target, dmg2minion);
            if (dmg2minion >= target.Hp && !target.divineshild && !target.immune)
            {
                damg2hero = dmg2minion - target.Hp;
            }
            p.minionGetDamageOrHeal(ownplay ? p.enemyHero : p.ownHero, damg2hero);
        }

    }
}