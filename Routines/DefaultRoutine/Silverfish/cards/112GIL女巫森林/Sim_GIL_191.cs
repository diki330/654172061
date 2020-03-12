namespace HREngine.Bots
{
	class Sim_GIL_191 : SimTemplate //* 恶魔法阵 Fiendish Circle
	{
        //[x]Summon four 1/1 Imps.
        //召唤四个1/1的小鬼。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.GIL_191t);
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, pos, ownplay, false);
            p.callKid(kid, pos, ownplay);
            p.callKid(kid, pos, ownplay);
            p.callKid(kid, pos, ownplay);
        }

    }
}