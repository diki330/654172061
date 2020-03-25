namespace HREngine.Bots
{
    class Sim_LOOT_017 : SimTemplate //* 黑暗契约 Dark Pact
    {
        //Destroy a friendly minion. Restore #4 Health to your hero.
        //消灭一个友方随从。为你的英雄恢复#4点生命值。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetDestroyed(target);
            int heal = (ownplay) ? p.getSpellHeal(4) : p.getEnemySpellHeal(4);
            p.minionGetDamageOrHeal(ownplay ? p.ownHero : p.enemyHero, -heal);
        }

    }
}