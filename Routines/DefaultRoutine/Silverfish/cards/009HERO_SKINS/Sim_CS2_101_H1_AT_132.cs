namespace HREngine.Bots
{
    class Sim_CS2_101_H1_AT_132 : SimTemplate //* 白银之手 The Silver Hand
    {
        //<b>Hero Power</b>Summon two 1/1 Recruits.
        //<b>英雄技能</b>召唤两个1/1的白银之手新兵。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_101t);//silverhandrecruit

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int place = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, place, ownplay, false);
            p.callKid(kid, place, ownplay);
        }

    }
}