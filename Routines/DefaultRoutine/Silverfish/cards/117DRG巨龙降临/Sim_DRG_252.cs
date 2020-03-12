namespace HREngine.Bots
{
	class Sim_DRG_252 : SimTemplate //* 相位追猎者 Phase Stalker
	{
		//[x]After you use your HeroPower, cast a <b>Secret</b>from your deck.
		//在你使用你的英雄技能后，从你的牌库中施放一个<b>奥秘</b>。
		public override void onInspire(Playfield p, Minion m, bool ownerOfInspire)
		{
            if (m.own)
            {        
                 p.ownSecretsIDList.Add(CardDB.cardIDEnum.EX1_554);                
            }
            else
            {              
                    if (p.enemySecretCount <= 4)
                    {
                        p.enemySecretCount++;
                        SecretItem si = Probabilitymaker.Instance.getNewSecretGuessedItem(p.getNextEntity(), p.ownHeroStartClass);                       
                        if (Settings.Instance.useSecretsPlayAround)
                        {
                            p.enemySecretList.Add(si);
                        }
                    }
            }
        }

	}
}