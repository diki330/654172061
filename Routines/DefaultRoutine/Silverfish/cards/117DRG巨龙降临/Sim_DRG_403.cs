namespace HREngine.Bots
{
    class Sim_DRG_403 : SimTemplate //* 喷灯破坏者 Blowtorch Saboteur
    {
        //<b>Battlecry:</b> Your opponent's next Hero Power costs (3).
        //<b>战吼：</b>你对手的下一个英雄技能的法力值消耗为（3）点。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own) p.enemyHeroPowerCostLessOnce = 3;
            else p.ownHeroPowerCostLessOnce = 3;
        }
    }
}