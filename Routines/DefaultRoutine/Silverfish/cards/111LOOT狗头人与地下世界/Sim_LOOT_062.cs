namespace HREngine.Bots
{
    class Sim_LOOT_062 : SimTemplate //* 狗头人隐士 Kobold Hermit
    {
        //<b>Battlecry:</b> Choose a basic Totem. Summon it.
        //<b>战吼：</b>选择一个基础图腾并召唤它。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_050);//Searing Totem

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(kid, own.zonepos, own.own);
        }

    }
}