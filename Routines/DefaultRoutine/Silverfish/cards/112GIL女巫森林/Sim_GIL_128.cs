namespace HREngine.Bots
{
	class Sim_GIL_128 : SimTemplate //* 艾莫莉丝 Emeriss
	{
        //<b>Battlecry:</b> Double the Attack and Health of all minions in your hand.
        //<b>战吼：</b>使你手牌中所有随从牌的攻击力和生命值翻倍。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.MOB)
                    {
                        hc.addattack += hc.card.Attack;
                        hc.addHp += hc.card.Health;
                        p.anzOwnExtraAngrHp += (hc.card.Attack + hc.card.Health);
                    }
                }
            }
        }

    }
}