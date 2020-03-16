namespace HREngine.Bots
{
    class Sim_GIL_654 : SimTemplate //* 战路 Warpath
    {
        //<b>Echo</b>Deal $1 damage to all minions.
        //<b>回响</b>对所有随从造成$1点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            p.allMinionsGetDamage(dmg);
        }

    }
}