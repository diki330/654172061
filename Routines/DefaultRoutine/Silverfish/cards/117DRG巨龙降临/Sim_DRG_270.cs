namespace HREngine.Bots
{
    class Sim_DRG_270 : SimTemplate //* 织法巨龙玛里苟斯 Malygos, Aspect of Magic
    {
        //[x]<b>Battlecry:</b> If you're holdinga Dragon, <b>Discover</b> an upgraded Mage spell.
        //<b>战吼：</b>如果你的手牌中有龙牌，便<b>发现</b>一张升级过的法师法术牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            foreach (Handmanager.Handcard hc in p.owncards)
            {
                if (hc.card.race == 24)
                {
                    p.drawACard(CardDB.cardName.unknown, own.own);
                }
            }
        }

    }
}