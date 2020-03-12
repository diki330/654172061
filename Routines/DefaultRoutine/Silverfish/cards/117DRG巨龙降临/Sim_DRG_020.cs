namespace HREngine.Bots
{
	class Sim_DRG_020 : SimTemplate //* 怪盗军需官 EVIL Quartermaster
	{
		//<b>Battlecry:</b> Add a <b>Lackey</b> to your hand. Gain 3 Armor.
		//<b>战吼：</b>将一张<b>跟班</b>牌置入你的手牌。获得3点护甲值。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
			p.drawACard(CardDB.cardName.unknown, own.own, true);
			Minion targetHero = own.own ? p.ownHero : p.enemyHero;
			p.minionGetDamageOrHeal(targetHero, 3);
			p.minionGetArmor(targetHero, 3);
		}

	}
}