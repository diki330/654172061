namespace HREngine.Bots
{
	class Sim_GIL_805 : SimTemplate //* 破棺者 Coffin Crasher
	{
        //<b>Deathrattle:</b> Summon a <b>Deathrattle</b> minion from your hand.
        //<b>亡语：</b>从你的手牌中召唤一个<b>亡语</b>随从。
        public override void onDeathrattle(Playfield p, Minion m)
        {
            if (m.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.deathrattle)
                    {
                        p.callKid(hc.card, p.owncards.Count, m.own);
                        p.removeCard(hc);
                        break;
                    }
                }
            }
            else p.callKid(CardDB.Instance.getCardData(CardDB.cardName.unknown), p.enemyMinions.Count, m.own);
        }

    }
}