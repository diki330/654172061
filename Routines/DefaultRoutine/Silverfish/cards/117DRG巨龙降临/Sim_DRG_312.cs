namespace HREngine.Bots
{
    class Sim_DRG_312 : SimTemplate //* 盆栽投手 Shrubadier
    {
        //<b>Battlecry:</b> Summon a 2/2 Treant.
        //<b>战吼：</b>召唤一个2/2的树人。

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_311t);//2/2的树人
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(kid, own.zonepos - 1, own.own);
        }

    }
}