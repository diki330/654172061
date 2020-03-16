namespace HREngine.Bots
{
    class Sim_DRG_270t7 : SimTemplate //* 玛里苟斯的烈焰风暴 Malygos's Flamestrike
    {
        //Deal $8 damage to all enemy minions.
        //对所有敌方随从造成$8点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(8) : p.getEnemySpellDamageDamage(8);
            p.allMinionOfASideGetDamage(!ownplay, dmg);
        }

    }
}