namespace HREngine.Bots
{
	class Sim_ULD_256 : SimTemplate //* 投入战斗 Into the Fray
	{
        //Give all <b>Taunt</b> minions in your hand +2/+2.
        //使你手牌中的所有<b>嘲讽</b>随从牌获得+2/+2。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.MOB && hc.card.tank)
                    {
                        hc.addattack += 2;
                        hc.addHp += 2;
                        p.anzOwnExtraAngrHp += 4;
                    }
                }
            }
        }

    }
}