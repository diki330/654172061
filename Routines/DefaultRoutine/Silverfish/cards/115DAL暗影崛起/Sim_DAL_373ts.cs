namespace HREngine.Bots
{
    class Sim_DAL_373ts : SimTemplate //* 急速射击 Rapid Fire
    {
        //Deal $1 damage.
        //造成$1点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}