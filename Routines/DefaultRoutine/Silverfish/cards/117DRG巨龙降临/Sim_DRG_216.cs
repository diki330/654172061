namespace HREngine.Bots
{
    class Sim_DRG_216 : SimTemplate //* 电涌风暴 Surging Tempest
    {
        //Has +1 Attack while you have <b>Overloaded</b> Mana Crystals.
        //当你有<b>过载</b>的法力水晶时，获得+1攻击力。
        public override void onAuraStarts(Playfield p, Minion m)
        {
            if (m.own && p.ueberladung > 0) p.minionGetBuffed(m, 1, 0);
        }

        public override void onAuraEnds(Playfield p, Minion m)
        {
            if (m.own && p.ueberladung == 0) p.minionGetBuffed(m, -1, 0);
        }

    }
}