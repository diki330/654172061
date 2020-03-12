namespace HREngine.Bots
{
	class Sim_ULD_167 : SimTemplate //* 染病的兀鹫 Diseased Vulture
	{
        //After your hero takes damage on your turn, summon a random3-Cost minion.
        //你的英雄在自己的回合受到伤害后，随机召唤一个法力值消耗为（3）点的随从。
        public override void onMinionGotDmgTrigger(Playfield p, Minion triggerEffectMinion, int anzOwnMinionsGotDmg, int anzEnemyMinionsGotDmg, int anzOwnHeroGotDmg, int anzEnemyHeroGotDmg)
        {
            int pos = (triggerEffectMinion.own) ? p.ownMinions.Count : p.enemyMinions.Count;
            if (p.ownHero.anzGotDmg > 0 && triggerEffectMinion.own)
            {
                p.callKid(p.getRandomCardForManaMinion(3), pos, triggerEffectMinion.own);
            }
            else if (p.enemyHero.anzGotDmg > 0 && !triggerEffectMinion.own)
            {
                p.callKid(p.getRandomCardForManaMinion(3), pos, triggerEffectMinion.own);
            }
        }

    }
}