namespace HREngine.Bots
{
	class Sim_ULD_429 : SimTemplate //* 猎人工具包 Hunter's Pack
	{
        //Add a random Hunter Beast, <b>Secret</b>, and weapon to your hand.
        //随机将一张猎人野兽牌，<b>奥秘</b>牌和武器牌分别置入你的手牌。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            for (int i = 0; i < 3; i++)
            {
                p.drawACard(CardDB.cardName.unknown, ownplay, true);
            }
        }

    }
}