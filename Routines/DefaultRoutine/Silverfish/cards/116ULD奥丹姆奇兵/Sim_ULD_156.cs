namespace HREngine.Bots
{
	class Sim_ULD_156 : SimTemplate //* 恐龙大师布莱恩 Dinotamer Brann
	{
        //<b>Battlecry:</b> If your deck has no duplicates, summon King Krush.
        //<b>战吼：</b>如果你的牌库里没有相同的牌，则召唤暴龙王克鲁什。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own && p.prozis.noDuplicates)
            {
                CardDB.Card krush = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_156t3);
                p.callKid(krush, own.zonepos, own.own);
            }
        }

    }
}