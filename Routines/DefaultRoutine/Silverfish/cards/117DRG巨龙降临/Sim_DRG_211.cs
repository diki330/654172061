using System;
using System.Collections.Generic;
using System.Text;

    namespace HREngine.Bots
    {
        class Sim_DRG_211 : SimTemplate//* 猎风巨龙 Squallhunter
        {
        //<b>法术伤害+2</b><b>过载：</b>（2）

        public override void getBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own) p.ueberladung += 2;
        }
        public override void onAuraStarts(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.spellpower+=2;
            }
            else
            {
                p.enemyspellpower+=2;
            }
        }

        public override void onAuraEnds(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.spellpower-=2;
            }
            else
            {
                p.enemyspellpower-=2;
            }
        }

    }
    }
    