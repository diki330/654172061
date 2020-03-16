using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_GIL_583 : SimTemplate //* 图腾啃食者 Totem Cruncher
    {
        //<b>Taunt</b><b>Battlecry:</b> Destroy your Totems. Gain +2/+2 for each destroyed.
        //<b>嘲讽，战吼：</b>摧毁你的所有图腾。每摧毁一个图腾，便获得+2/+2。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;
            int totemNum = 0;
            foreach (Minion m in temp)
            {
                if ((TAG_RACE)m.handcard.card.race == TAG_RACE.TOTEM)
                {
                    p.minionGetDestroyed(m);
                    totemNum++;
                }
            }
            if (totemNum >= 1) p.minionGetBuffed(own, totemNum * 2, totemNum * 2);
        }

    }
}