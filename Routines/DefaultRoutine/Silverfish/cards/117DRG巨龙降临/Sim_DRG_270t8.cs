namespace HREngine.Bots
{
    class Sim_DRG_270t8 : SimTemplate //* 玛里苟斯的寒冰箭 Malygos's Frostbolt
    {
        //Deal $3 damage to a character and <b>Freeze</b> it.
        //对一个角色造成$3点伤害，并使其<b>冻结</b>。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
            p.minionGetFrozen(target);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}