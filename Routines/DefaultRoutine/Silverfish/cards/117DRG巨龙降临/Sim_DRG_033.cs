namespace HREngine.Bots
{
	class Sim_DRG_033 : SimTemplate //* 烛火吐息 Candle Breath
	{
        //Draw 3 cards. Costs (3) less while you're holding a Dragon.
        //抽三张牌。如果你的手牌中有龙牌，这张牌的法力值消耗减少（3）点。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            /*
            bool dragonInHand = false;
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
                {
                    dragonInHand = true;
                    break;
                }
            }
            if (dragonInHand)
            {

            }
            */
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
        }
    }
}