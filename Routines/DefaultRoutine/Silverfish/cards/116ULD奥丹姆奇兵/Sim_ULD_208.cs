namespace HREngine.Bots
{
	class Sim_ULD_208 : SimTemplate //* 卡塔图防御者 Khartut Defender
	{
        //[x]<b>Taunt</b>, <b>Reborn</b><b>Deathrattle:</b> Restore #3Health to your hero.
        //<b>嘲讽，复生，亡语：</b>为你的英雄恢复#3点生命值。
        public override void onDeathrattle(Playfield p, Minion m)
        {
            int healValue = 3;
            healValue = m.own ? p.getMinionHeal(healValue) : p.getEnemyMinionHeal(healValue);
            var hero = m.own ? p.ownHero : p.enemyHero;
            p.minionGetDamageOrHeal(hero, -healValue, true);
        }

    }
}