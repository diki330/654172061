using System;
using System.Collections.Generic;
using System.Text;

namespace HREngine.Bots
{
    class Sim_DRG_069 : SimTemplate //* 破甲骑士 Platebreaker
    {
        //<b>Battlecry:</b> Destroy your opponent's Armor.
        //<b>战吼：</b>摧毁你对手的护甲。
        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            Minion targetHero = own.own ? p.ownHero : p.enemyHero;
            int dmg = targetHero.armor;
            p.minionGetDamageOrHeal(targetHero, dmg);
        }


    }
}
