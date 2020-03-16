namespace HREngine.Bots
{
    class Sim_GIL_635 : SimTemplate //* 教堂石像兽 Cathedral Gargoyle
    {
        //<b>Battlecry:</b> If you're holding a Dragon, gain <b>Taunt</b> and <b>Divine Shield</b>.
        //<b>战吼：</b>如果你的手牌中有龙牌，则获得<b>嘲讽</b>和<b>圣盾</b>。
        public override void getBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (m.own)
            {
                bool dragonInHand = false;
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if ((TAG_RACE)hc.card.race == TAG_RACE.DRAGON)
                    {
                        dragonInHand = true;
                        break;
                    }
                }
                if (dragonInHand)
                {
                    m.taunt = true;
                    m.divineshild = true;
                    p.anzOwnTaunt++;
                }
            }
            else
            {
                if (p.enemyAnzCards >= 2)
                {
                    m.taunt = true;
                    m.divineshild = true;
                    p.anzEnemyTaunt++;
                }
            }
        }

    }
}