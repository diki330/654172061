namespace HREngine.Bots
{
    class Sim_BOT_066 : SimTemplate //* 机械雏龙 Mechanical Whelp
    {
        //<b>Deathrattle:</b> Summon a 7/7 Mechanical Dragon.
        //<b>亡语：</b>召唤一个7/7的机械巨龙。
        public override void onDeathrattle(Playfield p, Minion m)
        {
            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BOT_066t);
            p.callKid(kid, m.zonepos - 1, m.own);
        }

    }
}