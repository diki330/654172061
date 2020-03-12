namespace HREngine.Bots
{
	class Sim_BOT_532 : SimTemplate //* 投弹机器人 Explodinator
	{
		//<b>Battlecry:</b> Summon two 0/2 Goblin Bombs.
		//<b>战吼：</b>召唤两个0/2的地精炸弹。
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BOT_031);
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.callKid(kid, own.zonepos - 1, own.own); //1st left
			p.callKid(kid, own.zonepos, own.own);
		}

	}
}