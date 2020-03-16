namespace HREngine.Bots
{
    class Sim_BOT_700 : SimTemplate //* 大铡蟹 SN1P-SN4P
    {
        //<b>Magnetic</b>, <b>Echo</b><b>Deathrattle:</b> Summon two 1/1 Microbots.
        //<b>磁力，回响，亡语：</b>召唤两个1/1的微型机器人。
        public override void onDeathrattle(Playfield p, Minion m)
        {
            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BOT_312t);
            p.callKid(kid, m.zonepos, m.own);
            p.callKid(kid, m.zonepos + 1, m.own);
        }

    }
}