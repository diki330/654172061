namespace HREngine.Bots
{
	class Sim_ULD_262 : SimTemplate //* 高阶祭司阿门特 High Priest Amet
	{
        //[x]Whenever you summon aminion, set its Health equalto this minion's.
        //每当你召唤一个随从，将其生命值设置为与本随从相同。
        public override void onMinionWasSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
        {
            if (triggerEffectMinion.own == summonedMinion.own && triggerEffectMinion.entitiyID != summonedMinion.entitiyID)
            {
                summonedMinion.Hp = triggerEffectMinion.Hp;
            }
        }

    }
}