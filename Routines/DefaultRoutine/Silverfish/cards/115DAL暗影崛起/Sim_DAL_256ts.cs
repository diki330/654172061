namespace HREngine.Bots
{
	class Sim_DAL_256ts : SimTemplate //* 森林的援助 The Forest's Aid
	{
        //Summon five 2/2 Treants.
        //召唤五个2/2的树人。
        CardDB.Card treant = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DAL_256t2);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(treant, pos, ownplay, false);
            p.callKid(treant, pos, ownplay);
            p.callKid(treant, pos, ownplay);
            p.callKid(treant, pos, ownplay);
            p.callKid(treant, pos, ownplay);
        }

    }
}