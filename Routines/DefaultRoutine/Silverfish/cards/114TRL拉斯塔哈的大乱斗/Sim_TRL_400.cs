namespace HREngine.Bots
{
    class Sim_TRL_400 : SimTemplate //* 裂魂残像 Splitting Image
    {
        //<b>Secret:</b> When one of your minions is attacked, summon a copy of it.
        //<b>奥秘：</b>当你的随从受到攻击时，召唤一个该随从的复制。
        public override void onSecretPlay(Playfield p, bool ownplay, Minion target, int number)
        {
            int pos = target.zonepos;
            p.callKid(target.handcard.card, pos + 1, ownplay);
            p.ownMinions[pos + 1].setMinionToMinion(target);
        }

    }
}