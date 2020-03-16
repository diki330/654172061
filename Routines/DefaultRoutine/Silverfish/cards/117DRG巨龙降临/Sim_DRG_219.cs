using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_219 : SimTemplate // Explosive Shot
    {
        //对一个随从造成$4点伤害。如果你的手牌中有龙牌，则同样对其相邻随从造成伤害。

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {


            bool dragonInHand = false;
            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
                {
                    dragonInHand = true;
                    break;
                }
            }
            int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
            int dmg1 = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
            List<Minion> temp = (target.own) ? p.ownMinions : p.enemyMinions;
            p.minionGetDamageOrHeal(target, dmg);
            foreach (Minion m in temp)
            {
                if (dragonInHand)
                {
                    if (m.zonepos + 1 == target.zonepos || m.zonepos - 1 == target.zonepos) m.getDamageOrHeal(dmg1, p, true, false); // isMinionAttack=true because it is extra damage (we calc clear lostDamage)
                }
            }

        }
    }
}