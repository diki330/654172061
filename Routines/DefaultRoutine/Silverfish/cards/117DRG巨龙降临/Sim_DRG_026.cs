namespace HREngine.Bots
{
    class Sim_DRG_026 : SimTemplate //* 疯狂巨龙死亡之翼 Deathwing, Mad Aspect
    {
        //<b>Battlecry:</b> Attack ALLother minions.
        //<b>战吼：</b>攻击所有其他随从。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            foreach (Minion m in p.ownMinions)
            {
                if (m.entitiyID != own.entitiyID) p.minionGetDestroyed(m);
            }
            foreach (Minion m in p.enemyMinions)
            {
                if (m.entitiyID != own.entitiyID) p.minionGetDestroyed(m);
            }
        }
    }
}