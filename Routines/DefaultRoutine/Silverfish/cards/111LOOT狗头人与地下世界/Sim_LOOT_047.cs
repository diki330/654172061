namespace HREngine.Bots
{
    class Sim_LOOT_047 : SimTemplate //* 树皮术 Barkskin
    {
        //Give a minion +3 Health. Gain 3 Armor.
        //使一个随从获得+3生命值。获得3点护甲值。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionGetBuffed(target, 0, 3);
            p.minionGetArmor(ownplay ? p.ownHero : p.enemyHero, 3);
        }

    }
}