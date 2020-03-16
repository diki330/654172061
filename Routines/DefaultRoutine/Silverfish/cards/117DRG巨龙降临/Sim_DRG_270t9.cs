namespace HREngine.Bots
{
    class Sim_DRG_270t9 : SimTemplate //* 玛里苟斯的火球术 Malygos's Fireball
    {
        //Deal $8 damage.
        //造成$8点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(8) : p.getEnemySpellDamageDamage(8);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}