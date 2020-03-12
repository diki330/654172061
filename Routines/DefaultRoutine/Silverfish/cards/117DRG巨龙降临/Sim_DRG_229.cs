namespace HREngine.Bots
{
	class Sim_DRG_229 : SimTemplate //* 青铜龙探险者 Bronze Explorer
	{
		//<b>Lifesteal</b><b>Battlecry:</b> <b>Discover</b> a Dragon.
		//<b>吸血</b><b>战吼：</b><b>发现</b>一张龙牌。
		public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own, true);
        }

	}
}