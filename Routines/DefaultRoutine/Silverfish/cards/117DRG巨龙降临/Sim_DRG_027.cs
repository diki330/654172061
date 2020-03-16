namespace HREngine.Bots
{
    class Sim_DRG_027 : SimTemplate //* 幽影潜藏者 Umbral Skulker
    {
        //[x]<b>Battlecry:</b> If you've <b>Invoked</b>twice, add 3 Coins toyour hand.
        //<b>战吼：</b>如果你已经<b>祈求</b>过两次，则将三个幸运币置入你的手牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.thecoin, own.own);
            p.drawACard(CardDB.cardName.thecoin, own.own);
            p.drawACard(CardDB.cardName.thecoin, own.own);
        }

    }
}