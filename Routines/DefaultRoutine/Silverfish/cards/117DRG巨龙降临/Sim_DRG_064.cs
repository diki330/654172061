namespace HREngine.Bots
{
	class Sim_DRG_064 : SimTemplate //* 祖达克仪祭师 Zul'Drak Ritualist
	{
      CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_116t);//whelp
//    ansturm/. kampfschrei:/ ruft zwei welplinge (1/1) für euren gegner herbei.
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice) 
		{

            int pos = (own.own) ? p.enemyMinions.Count : p.ownMinions.Count;
            p.callKid(kid, pos, !own.own);
            p.callKid(kid, pos, !own.own);
            p.callKid(kid, pos, !own.own);
		}
	}
}