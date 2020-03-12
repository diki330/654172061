namespace HREngine.Bots
{
	class Sim_ULD_293 : SimTemplate //* 云雾王子 Cloud Prince
	{
        //<b>Battlecry:</b> If you control a <b>Secret</b>, deal 6 damage.
        //<b>战吼：</b>如果你控制一个<b>奥秘</b>，则造成6点伤害。

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            var damage = 6;
            if (p.ownSecretsIDList.Count >= 1)
            {
                p.minionGetDamageOrHeal(target, damage);
            }
        }
    }
}