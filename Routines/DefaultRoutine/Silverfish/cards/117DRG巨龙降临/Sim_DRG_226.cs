namespace HREngine.Bots
{
    class Sim_DRG_226 : SimTemplate //* 琥珀看守者 Amber Watcher
    {
        //<b>Battlecry:</b> Restore #8 Health.
        //<b>战吼：</b>恢复#8点生命值。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            int heal = (own.own) ? p.getMinionHeal(8) : p.getEnemyMinionHeal(8);
            p.minionGetDamageOrHeal(target, -heal);
        }

    }
}