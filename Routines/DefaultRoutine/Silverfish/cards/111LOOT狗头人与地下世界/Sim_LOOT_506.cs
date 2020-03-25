using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_LOOT_506 : SimTemplate //* 符文之矛
    {
        // 在你的英雄攻击后,发现一张法术牌,并向随机目标施放.
        // Handled in triggerAMinionLosesDivineShield()

        CardDB.Card weapon = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.LOOT_506);

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(weapon, ownplay);
        }
    }
}