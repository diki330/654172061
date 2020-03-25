namespace HREngine.Bots
{
    class Sim_LOOT_008 : SimTemplate //* 心灵尖啸 Psychic Scream
    {
        //Shuffle all minions into your opponent's deck.
        //将所有随从洗入你对手的牌库。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                foreach (Minion m in p.ownMinions)
                    p.minionReturnToDeck(m, !ownplay);
                foreach (Minion mn in p.enemyMinions)
                    p.minionReturnToDeck(mn, !ownplay);
            }
            else
            {
                foreach (Minion m in p.ownMinions)
                    p.minionReturnToDeck(m, ownplay);
                foreach (Minion mn in p.enemyMinions)
                    p.minionReturnToDeck(mn, ownplay);
            }
        }

    }
}