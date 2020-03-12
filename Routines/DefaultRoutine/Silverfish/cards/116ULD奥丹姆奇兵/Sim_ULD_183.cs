namespace HREngine.Bots
{
	class Sim_ULD_183 : SimTemplate //* 阿努比萨斯战争使者 Anubisath Warbringer
	{
        //<b>Deathrattle:</b> Give all minions in your hand +3/+3.
        //<b>亡语：</b>使你手牌中的所有随从牌获得+3/+3。
        public override void onDeathrattle(Playfield p, Minion m)
        {
            if (m.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.MOB)
                    {
                        hc.addattack += 3;
                        hc.addHp += 3;
                        p.anzOwnExtraAngrHp += 6;
                    }
                }
            }
        }

    }
}