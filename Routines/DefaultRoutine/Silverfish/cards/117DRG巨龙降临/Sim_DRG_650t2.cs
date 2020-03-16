namespace HREngine.Bots
{
    class Sim_DRG_650t2 : SimTemplate //* 天降浩劫迦拉克隆 Galakrond, the Apocalypse
    {
        //[x]<b>Battlecry:</b> Draw 2 minions.Give them +4/+4.<i>(2)</i>
        //<b>战吼：</b>抽两张随从牌，使其获得+4/+4。<i>（2）</i>
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own);
            p.drawACard(CardDB.cardName.unknown, own.own);
            p.owncards[p.owncards.Count - 1].addattack += 4;
            p.owncards[p.owncards.Count - 1].addHp += 4;
            p.owncards[p.owncards.Count - 2].addattack += 4;
            p.owncards[p.owncards.Count - 2].addHp += 4;
            p.ownHero.armor += 5;
            p.setNewHeroPower(CardDB.cardIDEnum.DRG_238p, own.own);
        }

    }
}