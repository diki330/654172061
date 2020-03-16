namespace HREngine.Bots
{
    class Sim_DRG_321 : SimTemplate //* 火球滚滚 Rolling Fireball
    {
        //Deal $8 damage to a minion. Any excess damage continues tothe left or right.
        //对一个随从造成$8点伤害，超过其生命值的伤害将由左侧或右侧的随从承担。

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(8) : p.getEnemySpellDamageDamage(8);
            int extraDamage = dmg - target.Hp;
            p.minionGetDamageOrHeal(target, dmg);

            if (extraDamage > 0) //有伤害溢出
            {
                var minions = ownplay ? p.ownMinions : p.enemyMinions;
                Minion extraTarget = null;
                int position = target.zonepos;
                if (position >= 1) //至少2个随从,并且target至少是第二个
                {
                    extraTarget = minions[position - 1]; //选左边的随从
                }
                else
                {
                    //target是第一个随从
                    if (minions.Count >= 2) //至少2个随从
                    {
                        extraTarget = minions[1]; //选第二个随从
                    }
                }

                if (extraTarget != null)
                {
                    p.minionGetDamageOrHeal(extraTarget, extraDamage);
                }
            }
        }
    }
}