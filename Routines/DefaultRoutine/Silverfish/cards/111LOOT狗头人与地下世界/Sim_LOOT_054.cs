namespace HREngine.Bots
{
    class Sim_LOOT_054 : SimTemplate //* 分岔路口 Branching Paths
    {
        //[x]<b>Choose Twice -</b> Draw acard; Give your minions +1 Attack; Gain 6 Armor.
        //<b>选择两次：</b>抽一张牌；使你的所有随从获得+1攻击力；或者获得6点护甲值。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            for (int i = 0; i < 2; i++)
            {
                if (choice == 1 || (p.ownFandralStaghelm > 0 && ownplay))
                {
                    p.allMinionOfASideGetBuffed(ownplay, 1, 0);
                }
                if (choice == 2 || (p.ownFandralStaghelm > 0 && ownplay))
                {
                    if (ownplay) p.minionGetArmor(p.ownHero, 6);
                    else p.minionGetArmor(p.enemyHero, 6);
                }
                if (choice == 3 || (p.ownFandralStaghelm > 0 && ownplay))
                {
                    p.drawACard(CardDB.cardName.unknown, ownplay);
                }
            }
        }

    }
}