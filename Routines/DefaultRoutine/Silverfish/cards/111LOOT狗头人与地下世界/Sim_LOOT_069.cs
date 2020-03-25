namespace HREngine.Bots
{
    class Sim_LOOT_069 : SimTemplate //* 下水道爬行者 Sewer Crawler
    {
        //<b>Battlecry:</b> Summon a 2/3 Giant Rat.
        //<b>战吼：</b>召唤一个2/3的巨鼠。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_069t);

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(kid, own.zonepos, own.own);
        }

    }
}