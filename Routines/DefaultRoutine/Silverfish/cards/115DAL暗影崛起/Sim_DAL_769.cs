namespace HREngine.Bots
{
	class Sim_DAL_769 : SimTemplate //* 提振士气 Improve Morale
	{
        //[x]Deal $1 damageto a minion.If it survives, add a<b>Lackey</b> to your hand.
        //对一个随从造成$1点伤害。如果它依然存活，则将一张<b>跟班</b>牌置入你的手牌。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int damage = ownplay ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            p.minionGetDamageOrHeal(target, damage);
            if (target.Hp >= 1)
            {
                p.drawACard(CardDB.cardName.unknown, ownplay, true);
            }
        }

    }
}