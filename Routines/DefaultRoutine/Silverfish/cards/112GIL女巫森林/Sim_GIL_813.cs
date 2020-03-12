namespace HREngine.Bots
{
	class Sim_GIL_813 : SimTemplate //* 鲜活梦魇 Vivid Nightmare
	{
        //[x]Choose a friendly minion.Summon a copy of it with1 Health remaining.
        //选择一个友方随从，召唤一个该随从的复制，且剩余生命值为1点。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int pos = p.ownMinions.Count;
            if (pos < 7)
            {
                p.callKid(target.handcard.card, pos, ownplay);
                p.ownMinions[pos].setMinionToMinion(target);
                p.ownMinions[pos].Hp = 1;
            }
        }

    }
}