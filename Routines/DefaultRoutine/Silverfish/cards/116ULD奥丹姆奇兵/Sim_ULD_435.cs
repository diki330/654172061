namespace HREngine.Bots
{
	class Sim_ULD_435 : SimTemplate //* 纳迦沙漠女巫 Naga Sand Witch
	{
        //[x]<b>Battlecry:</b> Change the Costof spells in your hand to (5).
        //<b>战吼：</b>使你手牌中的法术牌法力值消耗变为（5）点。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.SPELL)
                    {
                        hc.manacost = 5;
                    }
                }
            }
        }

    }
}