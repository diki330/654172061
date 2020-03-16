using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_270t4 : SimTemplate //* 玛里苟斯的魔爆术(Malygos's Explosion)
    {
        // [x]Deal 2 damage to all enemy minions.
        // 对所有敌方随从造成2点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            p.allMinionOfASideGetDamage(!ownplay, dmg);
        }

    }


}