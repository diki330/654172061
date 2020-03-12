namespace HREngine.Bots
{
	class Sim_ULD_713 : SimTemplate //* 飞蝗虫群 Swarm of Locusts
	{
        //Summon seven 1/1 Locusts with <b>Rush</b>.
        //召唤七只1/1并具有<b>突袭</b>的蝗虫。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            CardDB.Card locust = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_430t);
            if (ownplay)
            {
                for (int i = 0; i < 7; i++)
                {
                    int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
                    p.callKid(locust, pos, ownplay);
                }
            }
        }

    }
}