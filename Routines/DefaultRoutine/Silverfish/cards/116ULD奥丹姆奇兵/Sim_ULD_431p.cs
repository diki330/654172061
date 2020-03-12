using System.Collections.Generic;

namespace HREngine.Bots
{
	class Sim_ULD_431p : SimTemplate //* 帝王裹布 Emperor Wraps
	{
        //[x]<b>Hero Power</b>Summon a 2/2 copyof a friendly minion.
        //<b>英雄技能</b>召唤一个友方随从的2/2的复制。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;
            int pos = temp.Count;
            if (pos < 7)
            {
                p.callKid(target.handcard.card, pos, ownplay);
                temp[pos].setMinionToMinion(target);
                p.minionSetAngrToX(target, 2);
                p.minionSetLifetoX(target, 2);
            }
        }

    }
}