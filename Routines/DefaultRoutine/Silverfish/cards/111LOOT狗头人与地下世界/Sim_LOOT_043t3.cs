namespace HREngine.Bots
{
    class Sim_LOOT_043t3 : SimTemplate //* 大型法术紫水晶 Greater Amethyst Spellstone
    {
        //<b>Lifesteal.</b> Deal $7 damage to a minion.
        //<b>吸血</b>对一个随从造成$7点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(7) : p.getEnemySpellDamageDamage(7);

            int oldHp = target.Hp;
            p.minionGetDamageOrHeal(target, dmg);
            if (oldHp > target.Hp) p.applySpellLifesteal(oldHp - target.Hp, ownplay);
        }

    }
}