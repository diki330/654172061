namespace HREngine.Bots
{
	class Sim_GIL_117 : SimTemplate //* 狼人憎恶 Worgen Abomination
	{
        //At the end of your turn, deal 2 damage to all other damaged minions.
        //在你的回合结束时，对所有其他受伤的随从造成2点伤害。
        public override void onTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            if (turnEndOfOwner == triggerEffectMinion.own)
            {
                foreach (Minion m in p.ownMinions)
                {
                    if (m.wounded) p.minionGetDamageOrHeal(m, 2);
                }
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.wounded) p.minionGetDamageOrHeal(m, 2);
                }
            }
        }

    }
}