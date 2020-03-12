namespace HREngine.Bots
{
	class Sim_GIL_683 : SimTemplate //* 沼泽飞龙 Marsh Drake
	{
		//<b>Battlecry:</b> Summon a 2/1 <b>Poisonous</b> Drakeslayer for your opponent.
		//<b>战吼：</b>为你的对手召唤一个2/1并具有<b>剧毒</b>的飞龙猎手。
		public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
		{
			CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.GIL_683t);
			int pos = (m.own) ? p.enemyMinions.Count : p.ownMinions.Count;
			p.callKid(kid, pos, !m.own);
		}


	}
}