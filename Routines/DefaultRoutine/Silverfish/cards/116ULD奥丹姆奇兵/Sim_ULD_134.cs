namespace HREngine.Bots
{
	class Sim_ULD_134 : SimTemplate //* 蜂群来袭 BEEEES!!!
	{
		//[x]Choose a minion.Summon four 1/1 Beesthat attack it.
		//选择一个随从。召唤四只1/1的蜜蜂攻击该随从。
		CardDB.Card bee = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_134t);
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			if (target.Hp >= 4)
			{
				for (int i = 0; i < 4; i++)
				p.minionGetDamageOrHeal(target, 1);
			}	
			else
			{

			}
		}
	}
}