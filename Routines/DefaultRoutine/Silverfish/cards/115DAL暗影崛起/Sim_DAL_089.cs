namespace HREngine.Bots
{
    class Sim_DAL_089 : SimTemplate //* 魔法订书匠 Spellbook Binder
    {
        //<b>Battlecry:</b> If you have <b>Spell Damage</b>, draw a card.
        //<b>战吼：</b>如果你拥有<b>法术伤害</b>，抽一张牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own && p.spellpower > 0)
            {
                p.drawACard(CardDB.cardName.unknown, own.own);
            }
            if (!own.own && p.enemyspellpower > 0)
            {
                p.drawACard(CardDB.cardName.unknown, own.own);
            }
        }

    }
}