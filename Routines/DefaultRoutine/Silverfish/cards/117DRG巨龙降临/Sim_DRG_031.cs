namespace HREngine.Bots
{
	class Sim_DRG_031 : SimTemplate //* 死金药剂师 Necrium Apothecary
	{
        //<b>Combo:</b> Draw a <b>Deathrattle</b> minion from your deck and gain its <b>Deathrattle</b>.
        //<b>连击：</b>从你的牌库中抽一张<b>亡语</b>随从牌并获得其<b>亡语</b>。

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (p.cardsPlayedThisTurn >= 1)
            {
                p.drawACard(CardDB.cardName.sylvanaswindrunner, target.own);
            }
        }

        public override void onDeathrattle(Playfield p, Minion m)
        {
            Minion target;
            if (m.own)
            {
                target = p.searchRandomMinion(p.enemyMinions, searchmode.searchLowestHP);
            }
            else
            {
                target = p.searchRandomMinion(p.ownMinions, searchmode.searchHighestHP);
                if (p.isOwnTurn && target != null && target.Ready) p.evaluatePenality += 5;
            }
            if (target != null) p.minionGetControlled(target, m.own, false);
        }

    }
}