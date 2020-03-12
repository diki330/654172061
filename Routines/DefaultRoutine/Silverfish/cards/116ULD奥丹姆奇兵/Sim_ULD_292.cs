namespace HREngine.Bots
{
	class Sim_ULD_292 : SimTemplate //* 绿洲涌动者 Oasis Surger
	{
        //<b>Rush</b><b>Choose One -</b> Gain +2/+2; or Summon a copy of this minion.
        //<b>突袭</b><b>抉择：</b>获得+2/+2；或召唤一个该随从的复制。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (choice == 1 || (p.ownFandralStaghelm > 0 && own.own))
            {
                p.minionGetBuffed(own, 2, 2);
            }
            if (choice == 2 || (p.ownFandralStaghelm > 0 && own.own))
            {
                p.callKid(own.handcard.card, own.zonepos, own.own);
                p.ownMinions[own.zonepos + 1].setMinionToMinion(own);
            }
        }

    }
}