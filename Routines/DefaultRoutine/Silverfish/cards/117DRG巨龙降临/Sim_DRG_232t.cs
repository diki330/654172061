namespace HREngine.Bots
{
	class Sim_DRG_232t : SimTemplate //* 真银圣剑 Truesilver Champion
	{
		//Whenever your hero attacks, restore #2 Health to it.
		//每当你的英雄进攻，便为其恢复#2点生命值。
		CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_097);
        //
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(card, ownplay);
        }

	}
}