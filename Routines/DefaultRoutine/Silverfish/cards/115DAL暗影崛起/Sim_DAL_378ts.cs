namespace HREngine.Bots
{
    class Sim_DAL_378ts : SimTemplate //* 猛兽出笼 Unleash the Beast
    {
        //Summon a 5/5 Wyvern with <b>Rush</b>.
        //召唤一只5/5并具有<b>突袭</b>的双足飞龙。
        CardDB.Card wyvern = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DAL_378t1);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(wyvern, pos, ownplay, false);
        }

    }
}