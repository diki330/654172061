namespace HREngine.Bots
{
	class Sim_ULD_724p : SimTemplate //* 方尖碑之眼 Obelisk's Eye
	{
        //<b>Hero Power</b>Restore #3 Health.If you target a minion, also give it +3/+3.
        //<b>英雄技能</b>恢复#3点生命值。如果你的目标是一个随从，则同时使其获得+3/+3。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int heal = 3;
            if (ownplay)
            {
                if (p.anzOwnAuchenaiSoulpriest > 0 || p.embracetheshadow > 0) heal = -heal;
                if (p.doublepriest >= 1) heal *= (3 * p.doublepriest);
            }
            else
            {
                if (p.anzEnemyAuchenaiSoulpriest >= 1) heal = -heal;
                if (p.enemydoublepriest >= 1) heal *= (3 * p.enemydoublepriest);
            }
            p.minionGetDamageOrHeal(target, -heal);

            if (target.handcard.card.type == CardDB.cardtype.MOB)
            {
                p.minionGetBuffed(target, 3, 3);
            }
        }

    }
}