using System;
using System.Collections.Generic;
using System.Text;

    namespace HREngine.Bots
    {
        class Sim_YOD_023 : SimTemplate
        {
        //砰砰战队
        //发现一张跟班牌，机械牌或龙牌。

        public override void onCardPlay(Playfield p, bool ownplay, Minion target, int choice)
                {
                    p.drawACard(CardDB.cardName.unknown, ownplay, true);
                }
        }
    }
    