namespace HREngine.Bots
{
    class Sim_CS2_083b_H1 : SimTemplate //* 匕首精通 Dagger Mastery
    {
        //<b>Hero Power</b>Equip a 1/2 Dagger.
        //<b>英雄技能</b>装备一把1/2的匕首。
        CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_082);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(weapon, ownplay);
        }

    }
}