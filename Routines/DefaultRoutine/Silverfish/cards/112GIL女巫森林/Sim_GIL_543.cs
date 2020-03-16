namespace HREngine.Bots
{
    class Sim_GIL_543 : SimTemplate //* 黑暗附体 Dark Possession
    {
        //Deal $2 damage to a friendly character. <b>Discover</b> a Demon.
        //对一个友方角色造成$2点伤害。<b>发现</b>一张恶魔牌。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            p.minionGetDamageOrHeal(target, dmg);
            p.drawACard(CardDB.cardName.unknown, ownplay, true);
        }

    }
}