namespace HREngine.Bots
{
	class Sim_DRG_218 : SimTemplate //* 堕落的元素师 Corrupt Elementalist
	{
		//<b>Battlecry:</b> <b>Invoke</b> Galakrond twice.
		//<b>战吼：</b><b>祈求</b>迦拉克隆两次。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.getGalakrondInvoke(own.own);
			p.getGalakrondInvoke(own.own);
		}


	}
}