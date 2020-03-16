namespace HREngine.Bots
{
    class Sim_DRG_318 : SimTemplate //* 梦境吐息 Breath of Dreams
    {
        //Draw a card. If you're holding a Dragon, gain an empty Mana Crystal.
        //抽一张牌。如果你的手牌中有龙牌，便获得一个空的法力水晶。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, ownplay);
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if (hc.card.race == 24)
                {
                    if (p.ownMaxMana < 10) p.ownMaxMana++;
                }
            }
        }

    }
}