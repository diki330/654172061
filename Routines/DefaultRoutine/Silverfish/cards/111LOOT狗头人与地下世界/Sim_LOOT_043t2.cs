namespace HREngine.Bots
{
    class Sim_LOOT_043t2 : SimTemplate //* 法术紫水晶 Amethyst Spellstone
    {
        //<b>Lifesteal.</b> Deal $5 damage to a minion. <i>(Take damage from your cards to upgrade.)</i>
        //<b>吸血</b>对一个随从造成$5点伤害。<i>（受到来自你的卡牌的伤害后升级。）</i>
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(5) : p.getEnemySpellDamageDamage(5);

            int oldHp = target.Hp;
            p.minionGetDamageOrHeal(target, dmg);
            if (oldHp > target.Hp) p.applySpellLifesteal(oldHp - target.Hp, ownplay);
        }

    }
}