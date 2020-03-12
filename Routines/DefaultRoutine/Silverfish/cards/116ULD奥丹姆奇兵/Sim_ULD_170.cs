namespace HREngine.Bots
{
	class Sim_ULD_170 : SimTemplate //* 武装胡蜂 Weaponized Wasp
	{
        //<b>Battlecry:</b> If you controla <b>Lackey</b>, deal 3 damage.
        //<b>战吼：</b>如果你控制一个<b>跟班</b>，造成3点伤害。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (target != null)
            {
                p.minionGetDamageOrHeal(target, 3);
            }
        }

    }
}