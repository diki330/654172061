namespace HREngine.Bots
{
	class Sim_DRG_034 : SimTemplate //* 偷渡者 Stowaway
	{
        //[x]<b>Battlecry:</b> If there are cardsin your deck that didn't startthere, draw 2 of them.
        //<b>战吼：</b>如果你的牌库中有对战开始时不在牌库中的牌，则抽取其中的两张。
        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            //需要计算随机牌数目，配合法多雷
            p.drawACard(CardDB.cardIDEnum.None, m.own);
            p.drawACard(CardDB.cardIDEnum.None, m.own);
        }
    }
}