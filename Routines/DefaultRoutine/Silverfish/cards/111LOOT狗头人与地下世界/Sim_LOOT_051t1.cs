namespace HREngine.Bots
{
    class Sim_LOOT_051t1 : SimTemplate //* 法术玉石 Jasper Spellstone
    {
        //Deal $4 damage to a minion. 3
        //对一个随从造成$4点伤害。3
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}