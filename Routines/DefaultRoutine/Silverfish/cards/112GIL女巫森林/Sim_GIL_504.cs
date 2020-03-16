namespace HREngine.Bots
{
    class Sim_GIL_504 : SimTemplate //* 女巫哈加莎 Hagatha the Witch
    {
        //<b>Battlecry:</b> Deal 3 damage to all minions.
        //<b>战吼：</b>对所有随从造成3点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.setNewHeroPower(CardDB.cardIDEnum.GIL_504h, ownplay); // 蛊惑(Bewitch)
            if (ownplay) p.ownHero.armor += 5;
            else p.enemyHero.armor += 5;
            int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
            p.allMinionsGetDamage(dmg);
        }

    }
}