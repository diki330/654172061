namespace HREngine.Bots
{
    class Sim_GIL_188 : SimTemplate //* 镰刀德鲁伊 Druid of the Scythe
    {
        //[x]<b>Choose One -</b> Transforminto a 4/2 with <b>Rush</b>;or a 2/4 with <b>Taunt</b>.
        //<b>抉择：</b>将该随从变形成为4/2并具有<b>突袭</b>；或者将该随从变形成为2/4并具有<b>嘲讽</b>。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            CardDB.Card druidofthescythe42 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.GIL_188t);
            CardDB.Card druidofthescythe24 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.GIL_188t2);
            CardDB.Card druidofthescythe44 = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.GIL_188t3);
            if (p.ownFandralStaghelm > 0)
            {
                p.minionTransform(own, druidofthescythe44);
            }
            else
            {
                if (choice == 1)
                {
                    p.minionTransform(own, druidofthescythe42);
                }
                else if (choice == 2)
                {
                    p.minionTransform(own, druidofthescythe24);
                }
            }
        }

    }
}