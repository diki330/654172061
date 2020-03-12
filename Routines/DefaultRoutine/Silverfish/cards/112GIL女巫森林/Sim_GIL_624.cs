namespace HREngine.Bots
{
	class Sim_GIL_624 : SimTemplate //* 暗夜徘徊者 Night Prowler
	{
        //<b>Battlecry:</b> If this is the only minion on the battlefield, gain +3/+3.
        //<b>战吼：</b>如果它是战场上的唯一一个随从，获得+3/+3。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                if (p.ownMinions.Count == 0) p.minionGetBuffed(own, 3, 3);
            }
            else
            {
                if (p.enemyMinions.Count == 0) p.minionGetBuffed(own, 3, 3);
            }
        }

    }
}