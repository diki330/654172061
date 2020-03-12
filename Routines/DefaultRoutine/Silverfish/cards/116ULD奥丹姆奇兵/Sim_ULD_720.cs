namespace HREngine.Bots
{
	class Sim_ULD_720 : SimTemplate //* 血誓雇佣兵 Bloodsworn Mercenary
	{
        //[x]<b>Battlecry</b>: Choose adamaged friendly minion.Summon a copy of it.
        //<b>战吼：</b>选择一个受伤的友方随从，召唤一个它的复制。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null && target.own && target.wounded)
            {
                int position = p.ownMinions.Count;
                p.callKid(own.handcard.card, position, true);
                p.ownMinions[position].setMinionToMinion(target);
            }
        }

    }
}