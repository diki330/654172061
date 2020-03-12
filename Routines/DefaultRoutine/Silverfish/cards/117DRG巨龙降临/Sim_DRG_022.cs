namespace HREngine.Bots
{
	class Sim_DRG_022 : SimTemplate //* 横冲直撞 Ramming Speed
	{
        //Force a minion to attack one of its neighbors.
        //迫使一个随从攻击相邻的一个随从。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            //attack right neightbor
            foreach (Minion m in p.enemyMinions)
            {
                if (m.zonepos + 1 == target.zonepos || m.zonepos - 1 == target.zonepos)
                {
                    p.minionAttacksMinion(target, m, true);
                }
            }    

        }

    }
}