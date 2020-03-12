namespace HREngine.Bots
{
	class Sim_DAL_077 : SimTemplate //* 毒鳍鱼人 Toxfin
	{
        //<b>Battlecry:</b> Give a friendly Murloc <b>Poisonous</b>.
        //<b>战吼：</b>使一个友方鱼人获得<b>剧毒</b>。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null && (TAG_RACE)target.handcard.card.race == TAG_RACE.MURLOC) 
            {
                target.poisonous = true;
            }
        }

    }
}