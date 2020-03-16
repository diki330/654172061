namespace HREngine.Bots
{
    class Sim_BOT_224 : SimTemplate //* 双生小鬼 Doubling Imp
    {
        //<b>Battlecry:</b> Summon a copy of this minion.
        //<b>战吼：</b>召唤该随从的一个复制。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(own.handcard.card, own.zonepos, own.own);
            p.ownMinions[own.zonepos + 1].setMinionToMinion(own);
        }

    }
}