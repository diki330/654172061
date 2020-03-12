using System;
using System.Collections.Generic;
using System.Text;


namespace HREngine.Bots
{

	class Sim_DRG_242 : SimTemplate //* 迦拉克隆之盾 Shield of Galakrond
	{
		//<b>Taunt</b><b>Battlecry:</b> <b>Invoke</b> Galakrond.
		//<b>嘲讽，战吼：</b><b>祈求</b>迦拉克隆。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.getGalakrondInvoke(own.own);
		}
	}
}
