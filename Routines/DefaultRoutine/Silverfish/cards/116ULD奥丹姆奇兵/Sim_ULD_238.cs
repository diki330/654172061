namespace HREngine.Bots
{
	class Sim_ULD_238 : SimTemplate //* 考古专家雷诺 Reno the Relicologist
	{
        //<b>Battlecry:</b> If your deck has no duplicates, deal 10 damage randomly split among all enemy minions.
        //<b>战吼：</b>如果你的牌库里没有相同的牌，则造成10点伤害，随机分配到所有敌方随从身上。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own && p.prozis.noDuplicates)
            {
                for (int i = 0; i < 10; i++)
                {
                    target = p.getEnemyCharTargetForRandomSingleDamage(1, true);
                    p.minionGetDamageOrHeal(target, 1);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    target = p.searchRandomMinion(p.ownMinions, searchmode.searchHighestAttack);
                    p.minionGetDamageOrHeal(target, 1);
                }
            }
        }

    }
}