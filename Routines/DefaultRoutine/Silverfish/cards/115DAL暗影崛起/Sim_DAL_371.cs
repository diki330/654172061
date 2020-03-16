namespace HREngine.Bots
{
    class Sim_DAL_371 : SimTemplate //* 标记射击 Marked Shot
    {
        //Deal $4 damage to a minion. <b>Discover</b> a spell.
        //对一个随从造成$4点伤害。<b>发现</b>一张法术牌。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
            p.minionGetDamageOrHeal(target, dmg);
            p.drawACard(CardDB.cardName.unknown, ownplay, true);
        }

    }
}