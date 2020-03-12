namespace HREngine.Bots
{
	class Sim_ULD_272 : SimTemplate //* 神圣涟漪 Holy Ripple
	{
        //Deal $1 damage to all enemies. Restore #1 Health to all friendly characters.
        //对所有敌人造成$1点伤害，为所有友方角色恢复#1点生命值。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            int heal = (ownplay) ? p.getSpellHeal(1) : p.getEnemySpellHeal(1);
            if (ownplay)
            {
                p.minionGetDamageOrHeal(p.ownHero, -heal);
                p.minionGetDamageOrHeal(p.enemyHero, dmg);
                foreach (Minion m in p.ownMinions)
                {
                    p.minionGetDamageOrHeal(m, -heal);
                }

                foreach (Minion m in p.enemyMinions)
                {
                    p.minionGetDamageOrHeal(m, dmg);
                }
            }
            else
            {
                p.minionGetDamageOrHeal(p.enemyHero, -heal);
                p.minionGetDamageOrHeal(p.ownHero, dmg);
                foreach (Minion m in p.enemyMinions)
                {
                    p.minionGetDamageOrHeal(m, -heal);
                }

                foreach (Minion m in p.ownMinions)
                {
                    p.minionGetDamageOrHeal(m, dmg);
                }
            }
        }

    }
}