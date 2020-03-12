using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_270t6 : SimTemplate //* 玛里苟斯的变形术(Malygos's Polymorph)
	{
		// [x]Transform a minion into a 1/1 Sheep.
		// 使一个随从变形成为1/1的绵羊。
		private CardDB.Card sheep = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_tk1);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionTransform(target, sheep);
        }

		
	}


}