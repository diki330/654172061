namespace HREngine.Bots
{
    class Sim_LOOT_043 : SimTemplate //* 小型法术紫水晶 Lesser Amethyst Spellstone
    {
        //<b>Lifesteal.</b> Deal $3 damage to a minion. <i>(Take damage from your cards to upgrade.)</i>
        //<b>吸血</b>对一个随从造成$3点伤害。<i>（受到来自你的卡牌的伤害后升级。）</i>
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);

            int oldHp = target.Hp;
            p.minionGetDamageOrHeal(target, dmg);
            if (oldHp > target.Hp) p.applySpellLifesteal(oldHp - target.Hp, ownplay);
        }

    }
}