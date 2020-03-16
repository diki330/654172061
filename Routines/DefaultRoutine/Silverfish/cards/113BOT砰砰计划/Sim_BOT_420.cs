namespace HREngine.Bots
{
    class Sim_BOT_420 : SimTemplate //* 植树造林 Landscaping
    {
        //Summon two 2/2 Treants.
        //召唤两个2/2的树人。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_158t);

            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, pos, ownplay);
            p.callKid(kid, pos, ownplay);
        }

    }
}