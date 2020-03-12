namespace HREngine.Bots
{
	class Sim_DAL_577 : SimTemplate //* 霜冻射线 Ray of Frost
	{
        //<b>Twinspell</b><b>Freeze</b> a minion.If it's already <b>Frozen</b>,deal $2 damage to it.
        //<b>双生法术</b><b>冻结</b>一个随从。如果该随从已被<b>冻结</b>，则对其造成$2点伤害。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int damage = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            if (target.frozen)
            {
                p.minionGetDamageOrHeal(target, damage);
            }
            else
            {
                p.minionGetFrozen(target);
            }
            p.drawACard(CardDB.cardIDEnum.DAL_577ts, ownplay, true);
        }

    }
}