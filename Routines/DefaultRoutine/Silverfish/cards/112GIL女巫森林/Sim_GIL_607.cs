namespace HREngine.Bots
{
	class Sim_GIL_607 : SimTemplate //* 毒药贩子 Toxmonger
	{
        //[x]Whenever you play a 1-Costminion, give it <b>Poisonous</b>.
        //每当你使用一张法力值消耗为（1）点的随从牌，使其获得<b>剧毒</b>。
        public override void onCardIsGoingToBePlayed(Playfield p, Handmanager.Handcard hc, bool wasOwnCard, Minion triggerEffectMinion)
        {
            if (triggerEffectMinion.own == wasOwnCard)
            {
                if (hc.card.type == CardDB.cardtype.MOB && hc.manacost == 1)
                {
                    triggerEffectMinion.poisonous = true;
                }
            }
        }

    }
}