using System.Collections.Generic;

namespace HREngine.Bots
{
    class Sim_DRG_067 : SimTemplate //* 巨魔蝙蝠骑士 Troll Batrider
    {
        // Battlecry: Deal 3 damage to a random enemy minion.  

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.enemyMinions : p.ownMinions;
            int times = (own.own) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);

            if (temp.Count >= 1)
            {
                //search Minion with lowest hp
                Minion enemy = temp[0];
                int minhp = 10000;
                foreach (Minion m in temp)
                {
                    if (m.Hp >= times + 1 && minhp > m.Hp)
                    {
                        enemy = m;
                        minhp = m.Hp;
                    }
                }

                p.minionGetDamageOrHeal(enemy, times);

            }
        }

    }

}