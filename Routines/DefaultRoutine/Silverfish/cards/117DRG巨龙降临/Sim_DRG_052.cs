namespace HREngine.Bots
{
    class Sim_DRG_052 : SimTemplate //* 龙族跟班 Draconic Lackey
    {
        //<b>Battlecry:</b> <b>Discover</b> a Dragon.
        //<b>战吼：</b><b>发现</b>一张龙牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own, true);
        }
    }
}