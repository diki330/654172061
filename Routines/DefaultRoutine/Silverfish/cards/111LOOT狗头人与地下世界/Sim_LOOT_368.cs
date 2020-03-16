namespace HREngine.Bots
{
    class Sim_LOOT_368 : SimTemplate //* 虚空领主 Voidlord
    {
        //[x]<b>Taunt</b> <b>Deathrattle:</b> Summon three1/3 Demons with <b>Taunt</b>.
        //<b>嘲讽，亡语：</b>召唤三个1/3并具有<b>嘲讽</b>的恶魔。
        public override void onDeathrattle(Playfield p, Minion m)
        {
            var card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_065);
            p.callKid(card, m.zonepos - 1, m.own, true, true);
            p.callKid(card, m.zonepos, m.own, true, true);
            p.callKid(card, m.zonepos + 1, m.own, true, true);
        }

    }
}