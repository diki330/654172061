namespace HREngine.Bots
{
    class Sim_DRG_217 : SimTemplate //* 巨龙的兽群 Dragon's Pack
    {
        //Summon two 2/3 Spirit Wolves with <b>Taunt</b>. If you've <b>Invoked</b> twice, give them +3/+3.
        //召唤两只2/3并具有<b>嘲讽</b>的幽灵狼。如果你已经<b>祈求</b>过两次，则使它们获得+2/+2。
        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DRG_217t);
        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int pos = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;

            p.minionGetBuffed(target, 2, 2);
            p.callKid(kid, pos, ownplay, false);
            p.callKid(kid, pos, ownplay);

            /*else 
			{ 
			p.callKid(kid, pos, ownplay, false);
			p.callKid(kid, pos, ownplay);
			}*/
        }

    }
}