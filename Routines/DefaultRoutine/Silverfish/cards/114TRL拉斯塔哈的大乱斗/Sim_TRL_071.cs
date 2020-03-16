namespace HREngine.Bots
{
    class Sim_TRL_071 : SimTemplate //* 血帆啸猴 Bloodsail Howler
    {
        //[x]<b>Rush</b><b>Battlecry:</b> Gain +1/+1for each other Pirateyou control.
        //<b>突袭</b>，<b>战吼：</b>你每控制一个其他海盗，便获得+1/+1。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                int otherpirate = 0;
                foreach (Minion m in p.ownMinions)
                {
                    if ((TAG_RACE)own.handcard.card.race == TAG_RACE.PIRATE) otherpirate++;
                }
                p.minionGetBuffed(own, otherpirate, otherpirate);
            }
        }

    }
}