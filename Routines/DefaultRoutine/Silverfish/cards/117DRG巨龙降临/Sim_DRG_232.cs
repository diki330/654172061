namespace HREngine.Bots
{
	class Sim_DRG_232 : SimTemplate //* 光铸狂热者 Lightforged Zealot
	{
		//<b>Battlecry:</b> If your deck has no Neutral cards, equip a   4/2 Truesilver Champion.
		//<b>战吼：</b>如果你的牌库中没有中立卡牌，便装备一把4/2的真银圣剑。
		CardDB.Card w = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_232t);

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.equipWeapon(w, true);
            p.equipWeapon(w, false);
        }

	}
}