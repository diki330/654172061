namespace HREngine.Bots
{
	class Sim_ULD_151 : SimTemplate //* 拉穆卡恒驯兽师 Ramkahen Wildtamer
	{
        //<b>Battlecry:</b> Copy a random Beast in your hand.
        //<b>战吼：</b>随机复制一张你手牌中的野兽牌。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                Handmanager.Handcard hc = p.searchRandomMinionInHand(p.owncards, searchmode.searchLowestCost, GAME_TAGs.CARDRACE, TAG_RACE.PET);
                if (hc != null) p.drawACard(hc.card.name, own.own, true);
            }
        }

    }
}