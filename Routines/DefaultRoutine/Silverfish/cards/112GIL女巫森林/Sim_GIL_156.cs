namespace HREngine.Bots
{
    class Sim_GIL_156 : SimTemplate //* 石英元素 Quartz Elemental
    {
        //Can't attack while damaged.
        //受伤时无法攻击。
        public override void onEnrageStart(Playfield p, Minion m)
        {
            m.cantAttack = false;
            m.updateReadyness();
        }
        public override void onEnrageStop(Playfield p, Minion m)
        {
            m.cantAttack = true;
            m.updateReadyness();
        }

    }
}