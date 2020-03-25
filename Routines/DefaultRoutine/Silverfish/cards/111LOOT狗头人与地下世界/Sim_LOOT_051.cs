namespace HREngine.Bots
{
    class Sim_LOOT_051 : SimTemplate //* 小型法术玉石 Lesser Jasper Spellstone
    {
        //Deal $2 damage to a minion. 3<i>(Gain 3 Armor to upgrade.)</i>
        //对一个随从造成$2点伤害。3<i>（获得3点护甲值后升级。）</i>
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}