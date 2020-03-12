namespace HREngine.Bots
{
    public class Sim_DAL_739 : SimTemplate
    {
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                p.minionGetBuffed(target, 1, 0);
                p.minionGetRush(target);
            }   
            else p.evaluatePenality += 10;
        }
    }
}
