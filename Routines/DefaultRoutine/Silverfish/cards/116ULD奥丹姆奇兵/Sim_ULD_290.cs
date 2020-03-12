namespace HREngine.Bots
{
	class Sim_ULD_290 : SimTemplate //* 历史爱好者 History Buff
	{
        //Whenever you play a minion, give a random minion in your hand +1/+1.
        //每当你使用一张随从牌，随机使你手牌中的一张随从牌获得+1/+1。
        public override void onMinionWasSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
        {
            if (summonedMinion.playedFromHand && summonedMinion.own == triggerEffectMinion.own && summonedMinion.entitiyID != triggerEffectMinion.entitiyID)
            {
                if (triggerEffectMinion.own)
                {
                    Handmanager.Handcard hc = p.searchRandomMinionInHand(p.owncards, searchmode.searchLowestCost, GAME_TAGs.Mob);
                    if (hc != null)
                    {
                        hc.addattack++;
                        hc.addHp++;
                        p.anzOwnExtraAngrHp += 2;
                    }
                }
            }
        }

    }
}