namespace HREngine.Bots
{
	class Sim_GIL_803 : SimTemplate //* 民兵指挥官 Militia Commander
	{
		//<b>Rush</b><b>Battlecry:</b> Gain +3 Attack this turn.
		//<b>突袭，战吼：</b>在本回合获得+3攻击力。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.minionGetTempBuff(own, 3, 0);
		}

	}
}