namespace HREngine.Bots
{
    class Sim_DRG_037 : SimTemplate //* 菲里克·飞刺 Flik Skyshiv
    {
        //[x]<b>Battlecry:</b> Destroy aminion and all copies of it<i>(wherever they are)</i>.
        //<b>战吼：</b>消灭一个随从及其所有的复制<i>（无论它们在哪）</i>。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            CardDB.cardIDEnum id = target.handcard.card.cardIDenum;

            //场上
            if (target != null) p.minionGetDestroyed(target);


            if (own.own)
            {
                //手牌中
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.cardIDenum == id) p.removeCard(hc);
                }

                //牌库 //可能无效
                foreach (var dc in p.prozis.turnDeck)
                {
                    if (dc.Key == id) p.prozis.turnDeck.Remove(dc.Key);
                    p.ownDeckSize--;
                }
            }



        }

    }
}