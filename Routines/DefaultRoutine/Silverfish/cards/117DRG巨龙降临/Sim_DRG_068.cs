namespace HREngine.Bots
{
    class Sim_DRG_068 : SimTemplate //* 活化龙息 Living Dragonbreath
    {
        //Your minions can't be <b>Frozen</b>.
        //你的随从无法被<b>冻结</b>。
        public override void onAuraStarts(Playfield p, Minion own)
        {
            if (own.own)
            {
                foreach (Minion m in p.ownMinions)
                {
                    if (m.frozen) m.frozen = false;
                }
            }
            else
            {
                foreach (Minion m in p.enemyMinions)
                {
                    if (m.frozen) m.frozen = false;
                }
            }
        }
    }
}