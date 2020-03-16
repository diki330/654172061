namespace HREngine.Bots
{
    class Sim_BOT_447 : SimTemplate //* 晶化师 Crystallizer
    {
        //[x]<b>Battlecry:</b> Deal 5 damageto your hero. Gain 5 Armor.
        //<b>战吼：</b>对你的英雄造成5点伤害。获得5点护甲值。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            Minion targetHero = own.own ? p.ownHero : p.enemyHero;
            p.minionGetDamageOrHeal(targetHero, 5);
            p.minionGetArmor(targetHero, 5);

        }

    }
}