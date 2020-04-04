namespace HREngine.Bots
{
    class Sim_CS2_056_H3 : SimTemplate //* 生命分流 Life Tap
    {
        //<b>Hero Power</b>Draw a card and take $2 damage.
        //<b>英雄技能</b>抽一张牌并受到$2点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.None, ownplay);

            int dmg = 2;
            if (ownplay)
            {
                if (p.doublepriest >= 1) dmg *= (2 * p.doublepriest);
                p.minionGetDamageOrHeal(p.ownHero, dmg);
            }
            else
            {
                if (p.enemydoublepriest >= 1) dmg *= (2 * p.enemydoublepriest);
                p.minionGetDamageOrHeal(p.enemyHero, dmg);
            }
        }

    }
}