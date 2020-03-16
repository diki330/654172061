namespace HREngine.Bots
{
    class Sim_GIL_677 : SimTemplate //* 面具收集者 Face Collector
    {
        //<b>Echo</b><b>Battlecry:</b> Add a random <b>Legendary</b> minion to your hand.
        //<b>回响，战吼：</b>随机将一张<b>传说</b>随从牌置入你的手牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own, true);
        }

    }
}