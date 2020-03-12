namespace HREngine.Bots
{
	class Sim_GIL_687 : SimTemplate //* 通缉令 WANTED!
	{
        //Deal $3 damage to a minion. If that kills it, add a Coin to your hand.
        //对一个随从造成$3点伤害。如果“通缉令”杀死该随从，将一个幸运币置入你的手牌。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
            if (dmg >= target.Hp && !target.divineshild && !target.immune)
            {
                p.drawACard(CardDB.cardName.thecoin, ownplay, true);
            }
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}