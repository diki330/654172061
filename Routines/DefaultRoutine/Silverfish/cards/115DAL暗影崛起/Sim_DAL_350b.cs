namespace HREngine.Bots
{
    class Sim_DAL_350b : SimTemplate //* 愈合之花 Healing Blossom
    {
        //Restore #5 Health.
        //恢复#5点生命值。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int heal = (ownplay) ? p.getSpellHeal(5) : p.getEnemySpellHeal(5);
            p.minionGetDamageOrHeal(target, -heal);
        }
    }
}