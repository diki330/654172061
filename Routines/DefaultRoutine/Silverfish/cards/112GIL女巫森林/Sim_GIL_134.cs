namespace HREngine.Bots
{
    class Sim_GIL_134 : SimTemplate //* 圣水 Holy Water
    {
        //Deal $4 damage to a minion. If that kills it, add a copy of it to your hand.
        //对一个随从造成$4点伤害。如果“圣水”杀死该随从，将一张该随从的复制置入你的手牌。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
            if (dmg >= target.Hp && !target.divineshild && !target.immune)
            {
                p.drawACard(target.handcard.card.name, ownplay, true);
            }
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}