namespace HREngine.Bots
{
	class Sim_ULD_162 : SimTemplate //* 怪盗征募官 EVIL Recruiter
	{
        //<b>Battlecry:</b> Destroy a friendly <b>Lackey</b> to summon a 5/5 Demon.
        //<b>战吼：</b>消灭一个友方<b>跟班</b>，召唤一个5/5的恶魔。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_162t);
                p.minionGetDestroyed(target);
                p.callKid(kid, own.zonepos, own.own);
            }
        }

    }
}