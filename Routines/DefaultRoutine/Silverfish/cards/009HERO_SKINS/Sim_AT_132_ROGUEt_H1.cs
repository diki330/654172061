namespace HREngine.Bots
{
    class Sim_AT_132_ROGUEt_H1 : SimTemplate //* 浸毒匕首 Poisoned Dagger
    {
        //
        //
        CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.AT_132_ROGUEt);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(weapon, ownplay);
        }

    }
}