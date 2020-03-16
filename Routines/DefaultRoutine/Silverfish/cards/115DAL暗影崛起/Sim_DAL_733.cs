namespace HREngine.Bots
{
    class Sim_DAL_733 : SimTemplate //* 守卫梦境之路 Dreamway Guardians
    {
        //Summon two 1/2 Dryads with <b>Lifesteal</b>.
        //召唤两个1/2并具有<b>吸血</b>的树妖。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DAL_733t);//水晶树妖

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, pos, ownplay, false);
            p.callKid(kid, pos, ownplay);
        }

    }
}