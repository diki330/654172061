namespace HREngine.Bots
{
    class Sim_TRL_352 : SimTemplate //* 舔舔魔杖 Likkim
    {
        //Has +2 Attack while you have <b>Overloaded</b> Mana Crystals.
        //当你有<b>过载</b>的法力水晶时，获得+2攻击力。
        CardDB.Card likkim = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TRL_352);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(likkim, ownplay);
            if (ownplay)
            {
                if (p.ueberladung > 0 || p.lockedMana > 0)
                {
                    p.minionGetBuffed(p.ownHero, 2, 0);
                    p.ownWeapon.Angr += 2;
                }
            }

        }

    }
}