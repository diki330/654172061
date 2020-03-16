namespace HREngine.Bots
{
    class Sim_DRG_021 : SimTemplate //* 仪式斩斧 Ritual Chopper
    {
        //<b>Battlecry:</b> <b>Invoke</b> Galakrond.
        //<b>战吼：</b><b>祈求</b>迦拉克隆。
        CardDB.Card w = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_021);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(w, ownplay);
            p.getGalakrondInvoke(ownplay);
        }

    }
}