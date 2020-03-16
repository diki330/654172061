namespace HREngine.Bots
{
    class Sim_DRG_314 : SimTemplate //* 空气栽培 Aeroponics
    {
        //Draw 2 cards.Costs (2) less for each Treant you control.
        //抽两张牌。你每控制一个树人，该牌的法力值消耗便减少（2）点。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, ownplay);
            p.drawACard(CardDB.cardName.unknown, ownplay);
        }

    }
}