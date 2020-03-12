namespace HREngine.Bots
{
	class Sim_DRG_025 : SimTemplate //* 海盗之锚 Ancharrr
	{
        //After your hero attacks, draw a Pirate from your deck.
        //在你的英雄攻击后，从你的牌库中抽一张海盗牌。
        CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_025);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(weapon, ownplay);
        }

    }
}