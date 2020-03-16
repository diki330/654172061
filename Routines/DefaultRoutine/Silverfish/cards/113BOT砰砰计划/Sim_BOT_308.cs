namespace HREngine.Bots
{
    class Sim_BOT_308 : SimTemplate //* 弹簧火箭犬 Spring Rocket
    {
        //<b>Battlecry:</b> Deal 2 damage.
        //<b>战吼：</b>造成2点伤害。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionGetDamageOrHeal(target, 2);
        }

    }
}