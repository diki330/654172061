namespace HREngine.Bots
{
    class Sim_LOOT_060 : SimTemplate //* 粉碎之手 Crushing Hand
    {
        //Deal $8 damage to a minion. <b><b>Overload</b>:</b> (3)
        //对一个随从造成$8点伤害。<b>过载：</b>（3）
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(8) : p.getEnemySpellDamageDamage(8);
            p.minionGetDamageOrHeal(target, dmg);
            if (ownplay) p.ueberladung += 3;
        }

    }
}