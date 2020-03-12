namespace HREngine.Bots
{
	class Sim_GIL_515 : SimTemplate //* 捕鼠人 Ratcatcher
	{
        //<b>Rush</b><b>Battlecry:</b> Destroy a friendly minion and gain its Attack and Health.
        //<b>突袭，战吼：</b>消灭一个友方随从，并获得其攻击力和生命值。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                int atkBuff = target.Angr;
                int hpBuff = target.Hp;
                p.minionGetDestroyed(target);
                p.minionGetBuffed(own, atkBuff, hpBuff);
            }
        }

    }
}