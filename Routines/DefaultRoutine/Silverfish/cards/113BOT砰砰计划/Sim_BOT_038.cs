using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_BOT_038 : SimTemplate //* 烟火技师 Fireworks Tech
    {
        //[x]<b>Battlecry:</b> Give a friendlyMech +1/+1. If it has<b>Deathrattle</b>, trigger it.
        //<b>战吼：</b>使一个友方机械获得+1/+1。如果它具有<b>亡语</b>，则将其触发。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                var card = target.handcard.card;
                if (card.race == (int)TAG_RACE.MECHANICAL)
                {
                    p.minionGetBuffed(target, 1, 1);
                }

                if (card.deathrattle)
                {
                    p.doDeathrattles(new List<Minion> { target });
                }
            }
        }

    }
}