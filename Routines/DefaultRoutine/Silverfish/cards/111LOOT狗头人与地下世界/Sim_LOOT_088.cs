namespace HREngine.Bots
{
    class Sim_LOOT_088 : SimTemplate //* 英勇药水 Potion of Heroism
    {
        //Give a minion <b>Divine Shield</b>.Draw a card.
        //使一个随从获得<b>圣盾</b>。抽一张牌。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            target.divineshild = true;
            p.drawACard(CardDB.cardName.unknown, ownplay);
        }

    }
}