namespace HREngine.Bots
{
	class Sim_DRG_099 : SimTemplate //* 克罗斯·龙蹄 Kronx Dragonhoof
	{
		//[x]<b>Battlecry:</b> Draw Galakrond.If you're already Galakrond,unleash a Devastation.
		//<b>战吼：</b>抽取迦拉克隆。如果你已经变为迦拉克隆，则释放一场灾难。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.drawACard(CardDB.cardName.无敌巨龙迦拉克隆,own.own);
		}

	}
}