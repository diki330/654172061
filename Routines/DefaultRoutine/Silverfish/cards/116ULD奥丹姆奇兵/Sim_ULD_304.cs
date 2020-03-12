namespace HREngine.Bots
{
	class Sim_ULD_304 : SimTemplate //* 法奥瑞斯国王 King Phaoris
	{
        //[x]<b>Battlecry:</b> For each spellin your hand, summon arandom minion of thesame Cost.
        //<b>战吼：</b>你手牌中每有一张法术牌，便召唤一个法力值消耗与法术牌相同的随机随从。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.SPELL)
                    {
                        p.callKid(p.getRandomCardForManaMinion(hc.manacost), p.ownMinions.Count, own.own);
                    }
                    if (p.ownMinions.Count == 7) break;
                }
            }
        }

    }
}