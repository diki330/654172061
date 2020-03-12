namespace HREngine.Bots
{
	class Sim_ULD_324 : SimTemplate //* 小鬼油膏 Impbalming
	{
        //Destroy a minion. Shuffle 3 Worthless Imps into your deck.
        //消灭一个随从。将三张“游荡小鬼”洗入你的牌库。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetDestroyed(target);
            if (ownplay)
            {
                p.ownDeckSize += 3;
            }
            else
            {
                p.enemyDeckSize += 3;
            }
        }

    }
}