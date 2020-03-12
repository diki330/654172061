namespace HREngine.Bots
{
	class Sim_TRL_157 : SimTemplate //* 走跳板 Walk the Plank
	{
        //Destroy an undamaged minion.
        //消灭一个未受伤的随从。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (target != null && !target.wounded) 
            {
                p.minionGetDestroyed(target);
            }
        }

    }
}