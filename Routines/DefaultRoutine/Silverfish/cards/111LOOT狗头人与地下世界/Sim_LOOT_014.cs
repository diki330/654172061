namespace HREngine.Bots
{
    class Sim_LOOT_014 : SimTemplate //* 狗头人图书管理员 Kobold Librarian
    {
        /// <summary>
        /// Battlecry: Draw a card. Deal 2 damage to your hero.
        /// 战吼：抽一张牌。对你的英雄造成2点伤害。
        /// </summary>
        /// <param name="p"></param>
        /// <param name="own"></param>
        /// <param name="target"></param>
        /// <param name="choice"></param>
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.minionGetDamageOrHeal(own.own ? p.ownHero : p.enemyHero, 2);
            p.drawACard(CardDB.cardName.unknown, own.own);
        }


    }
}