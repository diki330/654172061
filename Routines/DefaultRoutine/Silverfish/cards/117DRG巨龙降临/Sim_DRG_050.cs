namespace HREngine.Bots
{
    class Sim_DRG_050 : SimTemplate //* 虔信狂徒 Devoted Maniac
    {
        //<b>Rush</b><b>Battlecry:</b> <b>Invoke</b> Galakrond.
        //<b>突袭，战吼：</b><b>祈求</b>迦拉克隆。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.getGalakrondInvoke(own.own);
        }

    }
}