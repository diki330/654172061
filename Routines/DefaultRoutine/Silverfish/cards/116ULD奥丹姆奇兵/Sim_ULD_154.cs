namespace HREngine.Bots
{
	class Sim_ULD_154 : SimTemplate //* 土狼头领 Hyena Alpha
	{
        //[x]<b>Battlecry:</b> If you controla <b>Secret</b>, summon two2/2 Hyenas.
        //<b>战吼：</b>如果你控制一个<b>奥秘</b>，便召唤两只2/2的土狼。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            CardDB.Card hyena = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_154t);

            int secretNum = (own.own) ? p.ownSecretsIDList.Count : p.enemySecretCount;
            if (secretNum > 0)
            {
                p.callKid(hyena, own.zonepos - 1, own.own);
                p.callKid(hyena, own.zonepos, own.own);
            }
        }

    }
}