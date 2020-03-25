namespace HREngine.Bots
{
    class Sim_LOOT_091 : SimTemplate //* 小型法术珍珠 Lesser Pearl Spellstone
    {
        //Summon a 2/2 Spirit with <b>Taunt</b>. 3<i>(Restore 3 Health to upgrade.)</i>
        //召唤一个2/2并具有<b>嘲讽</b>的灵魂。3<i>（恢复3点生命值后升级。）</i>
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_091t);//守护之魂

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;

            p.callKid(kid, pos, ownplay, false);
        }

    }
}