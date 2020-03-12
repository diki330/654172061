namespace HREngine.Bots
{
	class Sim_ULD_135 : SimTemplate //* 隐秘绿洲 Hidden Oasis
	{
        //<b>Choose One</b> - Summon a 6/6 Ancient with <b>Taunt</b>; or Restore #12 Health.
        //<b>抉择：</b>召唤一棵6/6并具有<b>嘲讽</b>的古树；或恢复#12点生命值。
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            CardDB.Card ancient = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.ULD_135at);
            if (choice == 1 || (p.ownFandralStaghelm > 0 && ownplay))
            {
                int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
                p.callKid(ancient, pos, ownplay, false);
            }

            if (choice == 2 || (target != null && p.ownFandralStaghelm > 0 && ownplay))
            {
                int heal = (ownplay) ? p.getSpellHeal(12) : p.getEnemySpellHeal(12);
                p.minionGetDamageOrHeal(target, -heal);
            }
        }

    }
}