namespace HREngine.Bots
{
    class Sim_BOT_535 : SimTemplate //* 微机操控者 Microtech Controller
    {
        //<b>Battlecry:</b> Summon two 1/1 Microbots.
        //<b>战吼：</b>召唤两个1/1的微型机器人。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BOT_312t); //1/1 微型机器人

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(kid, own.zonepos - 1, own.own); //1st left
            p.callKid(kid, own.zonepos, own.own);
        }

    }
}