namespace HREngine.Bots
{
    class Sim_GIL_118 : SimTemplate //* 癫狂的医生 Deranged Doctor
    {
        //<b>Deathrattle:</b> Restore #8 Health to your hero.
        //<b>亡语：</b>为你的英雄恢复#8点生命值。
        public override void onDeathrattle(Playfield p, Minion m)
        {
            int healValue = 8;
            if (m.own)
            {
                healValue = p.getMinionHeal(healValue);
                p.minionGetDamageOrHeal(p.ownHero, -healValue, true);
            }
            else
            {
                healValue = p.getEnemyMinionHeal(healValue);
                p.minionGetDamageOrHeal(p.enemyHero, -healValue, true);
            }
        }

    }
}