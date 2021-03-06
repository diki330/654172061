namespace HREngine.Bots
{
    class Sim_DAL_177 : SimTemplate //* 咒术师的召唤 Conjurer's Calling
    {
        //<b>Twinspell</b>Destroy a minion. Summon 2 minions of the same Cost to replace it.
        //<b>双生法术</b>消灭一个随从。召唤两个法力值消耗相同的随从来替换它。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int pos = target.zonepos;
            int manaCost = target.handcard.manacost;
            p.minionGetDestroyed(target);
            p.callKid(p.getRandomCardForManaMinion(manaCost), pos, ownplay, true, true);
            p.callKid(p.getRandomCardForManaMinion(manaCost), pos + 1, ownplay, true, true);
            p.drawACard(CardDB.cardIDEnum.DAL_177ts, ownplay, true);
        }

    }
}