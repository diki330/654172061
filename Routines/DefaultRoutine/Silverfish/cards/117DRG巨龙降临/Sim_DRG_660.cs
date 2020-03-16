namespace HREngine.Bots
{
    class Sim_DRG_660 : SimTemplate //* 讳言巨龙迦拉克隆 Galakrond, the Unspeakable
    {
        //[x]<b>Battlecry:</b> Destroy 1random enemy minion.<i>(2)</i>
        //<b>战吼：</b>随机消灭一个敌方随从。<i>（2）</i>
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            Minion m = p.searchRandomMinion(ownplay ? p.enemyMinions : p.ownMinions, searchmode.searchLowestHP);
            if (m != null) p.minionGetDestroyed(m);
            if (ownplay)
            {
                p.ownHero.armor += 5;
            }
            else
            {
                p.enemyHero.armor += 5;
            }
        }
    }
}