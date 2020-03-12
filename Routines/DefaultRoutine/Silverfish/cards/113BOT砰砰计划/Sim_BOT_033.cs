namespace HREngine.Bots
{
	class Sim_BOT_033 : SimTemplate //* 投掷炸弹 Bomb Toss
	{
        //Deal $2 damage. Summon a 0/2 Goblin Bomb.
        //造成$2点伤害。召唤一个0/2的地精炸弹。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BOT_031);

            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            p.minionGetDamageOrHeal(target, dmg);

            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, pos, ownplay);
        }

    }
}