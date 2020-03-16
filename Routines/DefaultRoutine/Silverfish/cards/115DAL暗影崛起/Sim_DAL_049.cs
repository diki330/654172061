namespace HREngine.Bots
{
    class Sim_DAL_049 : SimTemplate //* 下水道渔人 Underbelly Angler
    {
        //After you play a Murloc, add a random Murloc to your hand.
        //在你使用一张鱼人牌后，随机将一张鱼人牌置入你的手牌。
        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool wasOwnCard, Minion triggerEffectMinion)
        {
            if (triggerEffectMinion.own == wasOwnCard)
            {
                int murlocEnumValue = (int)TAG_RACE.MURLOC;
                if (hc.card.race == murlocEnumValue)
                {
                    //Bluegill Warrior 蓝腮战士
                    p.drawACard(CardDB.cardIDEnum.CS2_173, wasOwnCard, true);
                }
            }
        }

    }
}