using System;

namespace HREngine.Bots
{
    class Sim_DAL_060 : SimTemplate //* 发条地精 Clockwork Goblin
    {
        //[x]<b>Battlecry:</b> Shuffle a Bombinto your opponent's deck.When drawn, it explodesfor 5 damage.
        //<b>战吼：</b>将一张“炸弹” 牌洗入你对手的牌库。当玩家抽到“炸弹”时，便会受到5点伤害。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                p.enemyDeckSize++;
                if (p.enemyDeckSize <= 6)
                {
                    p.minionGetDamageOrHeal(p.enemyHero, Math.Min(5, p.enemyHero.Hp - 1), true);
                    p.evaluatePenality -= 1;
                }
                else
                {
                    if (p.enemyDeckSize <= 16)
                    {
                        p.minionGetDamageOrHeal(p.enemyHero, Math.Min(3, p.enemyHero.Hp - 1), true);
                        p.evaluatePenality -= 2;
                    }
                    else
                    {
                        if (p.enemyDeckSize <= 26)
                        {
                            p.minionGetDamageOrHeal(p.enemyHero, Math.Min(1, p.enemyHero.Hp - 1), true);
                            p.evaluatePenality -= 3;
                        }
                    }
                }
            }
            else
            {
                p.ownDeckSize++;
            }
        }

    }
}