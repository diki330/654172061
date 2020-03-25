namespace HREngine.Bots
{
    class Sim_LOOT_077 : SimTemplate //* 侧翼打击 Flanking Strike
    {
        //Deal $3 damage to a minion. Summon a 3/3 Wolf.
        //对一个随从造成$3点伤害。召唤一只3/3的狼。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_077t);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
            p.minionGetDamageOrHeal(target, dmg);

            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, pos, ownplay);
        }

    }
}