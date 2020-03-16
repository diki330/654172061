using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_249 : SimTemplate //* 祈求觉醒 Awaken!
    {
        //<b>Invoke</b> Galakrond. Deal $1 damage to all minions.
        //<b>祈求</b>迦拉克隆。对所有随从造成$1点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.getGalakrondInvoke(ownplay);
            int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            p.allMinionsGetDamage(dmg);

        }

    }
}