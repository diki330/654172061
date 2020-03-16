namespace HREngine.Bots
{
    class Sim_DAL_606 : SimTemplate //* 怪盗天才 EVIL Genius
    {
        //<b>Battlecry:</b> Destroy a friendly minion to add 2 random <b>Lackeys</b> to your hand.
        //<b>战吼：</b>消灭一个友方随从，随机将两张<b>跟班</b>牌置入你的手牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target == null)
            {
                return;
            }
            p.minionGetDestroyed(target);
            for (int i = 1; i <= 2; i++)
            {
                p.drawACard(CardDB.cardName.unknown, own.own, true);
            }
        }

    }
}