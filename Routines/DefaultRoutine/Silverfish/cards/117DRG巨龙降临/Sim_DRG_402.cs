using System.Collections.Generic;

namespace HREngine.Bots
{
	class Sim_DRG_402 : SimTemplate //* 萨索瓦尔 Sathrovarr
	{
        //<b>Battlecry:</b> Choose a friendly minion. Add a copy of it to your hand, deck, and battlefield.
        //<b>战吼：</b>选择一个友方随从。将它的一个复制置入你的手牌，牌库以及战场。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            int pos = temp.Count;
            if (pos < 7)
            {
                p.callKid(target.handcard.card, pos, own.own);
                temp[pos].setMinionToMinion(target);
                if (target != null)
                {
                    if (own.own)
                    {
                        p.drawACard(target.handcard.card.name, own.own, true);
                        p.ownDeckSize++;
                    }
                    else
                    {
                        p.enemyDeckSize++;
                    }
                }
            }
        }

    }
}