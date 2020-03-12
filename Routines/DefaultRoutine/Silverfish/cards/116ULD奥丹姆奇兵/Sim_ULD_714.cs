namespace HREngine.Bots
{
	class Sim_ULD_714 : SimTemplate //* 苦修 Penance
	{
        //<b>Lifesteal</b>Deal $3 damage to a minion.
        //<b>吸血</b>对一个随从造成$3点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);

            int oldHp = target.Hp;
            p.minionGetDamageOrHeal(target, dmg);
            if (oldHp > target.Hp) p.applySpellLifesteal(oldHp - target.Hp, ownplay);
        }

    }
}