namespace HREngine.Bots
{
	class Sim_DRG_301 : SimTemplate //* 怪盗低语 Whispers of EVIL
	{
		//Add a <b>Lackey</b> to your hand.
		//将一张<b>跟班</b>牌置入你的手牌。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			p.drawACard(CardDB.cardIDEnum.None, ownplay);
		}

	}
}