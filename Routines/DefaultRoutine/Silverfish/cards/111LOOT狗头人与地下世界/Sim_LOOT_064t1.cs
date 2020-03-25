using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_LOOT_064t1 : SimTemplate //* 法术蓝宝石 Sapphire Spellstone
    {
        //Summon 2 copies of a friendly minion. 3
        //选择一个友方随从，召唤两个它的复制。3
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;
            int pos = temp.Count;
            if (pos < 6)
            {
                p.callKid(target.handcard.card, pos, ownplay);
                p.callKid(target.handcard.card, pos, ownplay);
                temp[pos].setMinionToMinion(target);
            }

        }
    }
}