namespace HREngine.Bots
{
    class Sim_DRG_202 : SimTemplate //* 龙骨荒野邪教徒 Dragonblight Cultist
    {
        //[x]<b>Battlecry:</b> <b>Invoke</b> Galakrond.Gain +1 Attack for eachother friendly minion.
        //<b>战吼：</b><b>祈求</b>迦拉克隆。每有一个其他友方随从，便获得+1攻击力。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.getGalakrondInvoke(own.own);
            p.minionGetBuffed(own, (own.own ? p.ownMinions.Count - 1 : p.enemyMinions.Count - 1), 0);
        }



    }
}