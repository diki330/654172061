namespace HREngine.Bots
{
    class Sim_DRG_600 : SimTemplate //* 邪火巨龙迦拉克隆 Galakrond, the Wretched
    {
        //[x]<b>Battlecry:</b> Summon1 random Demon.<i>(2)</i>
        //<b>战吼：</b>随机召唤一个恶魔。<i>（2）</i>
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_415t3t);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int posi = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            if (ownplay)
            {
                p.ownHero.armor += 5;
                p.callKid(kid, posi, ownplay);
            }
            else
            {
                p.enemyHero.armor += 5;
                p.callKid(kid, posi, ownplay);
            }
        }
    }
}