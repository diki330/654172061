namespace HREngine.Bots
{
    class Sim_GIL_801 : SimTemplate //* 急速冷冻 Snap Freeze
    {
        //<b>Freeze</b> a minion.If it's already <b>Frozen</b>, destroy it.
        //<b>冻结</b>一个随从。如果该随从已被<b>冻结</b>，则将其消灭。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (target.frozen)
            {
                p.minionGetDestroyed(target);
            }
            else
            {
                p.minionGetFrozen(target);
            }
        }

    }
}