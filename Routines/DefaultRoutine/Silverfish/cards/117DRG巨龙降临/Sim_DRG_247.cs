using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
	class Sim_DRG_247 : SimTemplate //* 封印命运 Seal Fate
	{
		//Deal $3 damage to an undamaged character. <b>Invoke</b> Galakrond.
		//对一个未受伤的角色造成$3点伤害。<b>祈求</b>迦拉克隆。
		public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
			int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
			p.minionGetDamageOrHeal(target, dmg);
			p.getGalakrondInvoke(ownplay);
		}

	}
}