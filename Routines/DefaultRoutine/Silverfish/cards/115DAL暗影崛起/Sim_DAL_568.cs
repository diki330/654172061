namespace HREngine.Bots
{
    class Sim_DAL_568 : SimTemplate //* 光铸祝福 Lightforged Blessing
    {
        //<b>Twinspell</b>Give a friendly minion <b>Lifesteal</b>.
        //<b>双生法术</b>使一个友方随从获得<b>吸血</b>。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            target.lifesteal = true;
            p.drawACard(CardDB.cardIDEnum.DAL_568ts, ownplay, true);
        }

    }
}