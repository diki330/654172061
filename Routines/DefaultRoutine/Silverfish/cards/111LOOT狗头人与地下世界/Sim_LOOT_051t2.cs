namespace HREngine.Bots
{
    class Sim_LOOT_051t2 : SimTemplate //* 大型法术玉石 Greater Jasper Spellstone
    {
        //Deal $6 damage to a minion.
        //对一个随从造成$6点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(6) : p.getEnemySpellDamageDamage(6);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}