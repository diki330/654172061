namespace HREngine.Bots
{
    class Sim_DRG_078 : SimTemplate //* 深潜炸弹 Depth Charge
    {
        //At the start of your turn, deal 5 damage to ALL minions.
        //在你的回合开始时，对所有随从造成5点伤害。
        public override void onTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (turnStartOfOwner == triggerEffectMinion.own)
            {
                foreach (Minion m in p.ownMinions)
                {
                    if (m.entitiyID == triggerEffectMinion.entitiyID) continue;
                    if (m.playedThisTurn || m.playedPrevTurn)
                    {
                        if (PenalityManager.Instance.ownSummonFromDeathrattle.ContainsKey(m.name)) continue;
                        p.evaluatePenality += (m.Hp * 2 + m.Angr * 2) * 2;
                    }
                }
                p.allMinionsGetDamage(5);
            }
        }

    }
}