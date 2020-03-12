namespace HREngine.Bots
{
	class Sim_DRG_650t3 : SimTemplate //* 世界末日迦拉克隆 Galakrond, Azeroth's End
	{
        //[x]<b>Battlecry:</b> Draw 4 minions.Give them +4/+4.Equip a 5/2 Claw.
        //<b>战吼：</b>抽四张随从牌，使其获得+4/+4。装备一只5/2的巨爪。
        CardDB.Card w = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_112);
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own);
            p.drawACard(CardDB.cardName.unknown, own.own);
            p.drawACard(CardDB.cardName.unknown, own.own);
            p.drawACard(CardDB.cardName.unknown, own.own);
            p.owncards[p.owncards.Count - 1].addattack += 4;
            p.owncards[p.owncards.Count - 1].addHp += 4;
            p.owncards[p.owncards.Count - 2].addattack += 4;
            p.owncards[p.owncards.Count - 2].addHp += 4;
            p.owncards[p.owncards.Count - 3].addattack += 4;
            p.owncards[p.owncards.Count - 3].addHp += 4;
            p.owncards[p.owncards.Count - 4].addattack += 4;
            p.owncards[p.owncards.Count - 4].addHp += 4;
            p.equipWeapon(w, own.own);
            p.ownHero.armor += 5;
            p.setNewHeroPower(CardDB.cardIDEnum.DRG_238p, own.own);
        }

    }
}