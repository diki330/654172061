using System.Linq;

namespace HREngine.Bots
{
    class Sim_DRG_322 : SimTemplate //* 乘龙法师 Dragoncaster
    {
        //<b>Battlecry:</b> If you're holding a Dragon, your next spell this turn costs (0).
        //<b>战吼：</b>如果你的手牌中有龙牌，在本回合中你所施放的下一个法术的法力值消耗为（0）点。

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                var hasDragonInHand = p.owncards.Any(x => x.card.race == (int)TAG_RACE.DRAGON);
                p.nextSpellThisTurnCost0 = hasDragonInHand;
            }
        }
    }
}